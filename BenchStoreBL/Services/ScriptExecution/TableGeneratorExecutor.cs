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

        public async Task<string> ExecuteTableGenerator(string hostUrl, IEnumerable<string> resultFilePaths, IEnumerable<string>? logFilesPaths = null)
        {
            if (!resultFilePaths.Any())
            {
                throw new ArgumentException($"No result paths were provided!");
            }

            List<OptionArgument> optionArguments = new List<OptionArgument>
            {
                new OptionArgument
                {
                    Option = "--format",
                    Argument = "html",
                },
                new OptionArgument
                {
                    Option = "--outputpath",
                    Argument = "-",
                }
            };

            optionArguments.AddRange(resultFilePaths.Select(resultPath => new OptionArgument { Argument = $"{hostUrl}/{resultPath}" }));

            (string stdout, string stderr) = await _scriptExecutor.RunScript(_tableGeneratorOptions.TableGeneratorPath, optionArguments);
            if (string.IsNullOrEmpty(stdout))
            {
                throw new ArgumentException(stderr);
            }

            return stdout;
        }
    }
}

