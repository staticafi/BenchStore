using BenchStoreBL.Options;

using Microsoft.Extensions.Options;

namespace BenchStoreBL.Services.ScriptExecution
{
    internal class TableGeneratorExecutor : ITableGeneratorExecutor
    {
        private readonly IScriptExecutor _scriptExecutor;
        private readonly TableGeneratorOptions _tableGeneratorOptions;

        public TableGeneratorExecutor(IScriptExecutor scriptExecutor, IOptions<TableGeneratorOptions> options)
        {
            _scriptExecutor = scriptExecutor;
            _tableGeneratorOptions = options.Value;
        }

        public async Task<string> ExecuteTableGenerator(IEnumerable<string> resultFilePaths, IEnumerable<string>? logFilesPaths = null)
        {
            if (resultFilePaths.Count() == 0)
            {
                throw new ArgumentException($"No paths were provided!");
            }

            string tableGeneratorPath = _tableGeneratorOptions.TableGeneratorPath;

            List<OptionArgument> optionArguments = new List<OptionArgument>();
            string tmpPath = Directory.CreateTempSubdirectory().FullName;
            string tmpFileName = Guid.NewGuid().ToString();

            if (logFilesPaths != null)
            {
                foreach (string logFilesPath in logFilesPaths)
                {
                    string tmpLogName = Path.ChangeExtension(Guid.NewGuid().ToString(), ".zip");
                    string tmpLogPath = Path.Combine(tmpPath, tmpLogName);

                    if (!File.Exists(logFilesPath))
                    {
                        throw new Exception();
                    }

                    using (var writer = File.Create(tmpLogPath))
                    {
                        using (var reader = File.OpenRead(logFilesPath))
                        {
                            await reader.CopyToAsync(writer);
                        }
                    }
                }
            }

            optionArguments.Add(new OptionArgument
            {
                Option = "--outputpath",
                Argument = tmpPath,
            });

            optionArguments.Add(new OptionArgument
            {
                Option = "--name",
                Argument = tmpFileName,
            });

            optionArguments.Add(new OptionArgument
            {
                Option = "--format",
                Argument = "html",
            });

            optionArguments.AddRange(resultFilePaths
                .Select(x => new OptionArgument { Argument = x }));

            await _scriptExecutor.RunScript(tableGeneratorPath, optionArguments);

            string outputPath = Path.Combine(tmpPath, tmpFileName);
            if (resultFilePaths.Count() > 1)
            {
                outputPath = Path.ChangeExtension(outputPath, ".table.html");
            }
            else
            {
                outputPath = Path.ChangeExtension(outputPath, ".html");
            }

            return outputPath;
        }
    }
}

