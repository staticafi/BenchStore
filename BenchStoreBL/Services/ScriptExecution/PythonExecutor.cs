using System.Diagnostics;
using System.Text;

using BenchStoreBL.Options;

using Microsoft.Extensions.Options;

namespace BenchStoreBL.Services.ScriptExecution
{
    internal class PythonExecutor : IScriptExecutor
    {
        private readonly TableGeneratorOptions _tableGeneratorOptions;

        public PythonExecutor(IOptions<TableGeneratorOptions> options)
        {
            _tableGeneratorOptions = options.Value;
        }

        public async Task<(string, string)> RunScript(string command, IEnumerable<OptionArgument> arguments)
        {
            string pythonPath = _tableGeneratorOptions.PythonPath;

            string combinedOptionArguments = CombineOptionArguments(arguments);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = pythonPath;
                process.StartInfo.Arguments = $"\"{command}\" {combinedOptionArguments}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                string stdout = await process.StandardOutput.ReadToEndAsync();
                string stderr = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();
                return (stdout, stderr);
            }
        }

        private string CombineOptionArguments(IEnumerable<OptionArgument> optionArguments)
        {
            var stringBuilder = new StringBuilder();

            foreach (var optionArgument in optionArguments)
            {
                if (optionArgument.Option != null)
                {
                    stringBuilder.Append($"{optionArgument.Option} ");
                }

                stringBuilder.Append($"\"{optionArgument.Argument}\" ");
            }

            return stringBuilder.ToString();
        }
    }
}

