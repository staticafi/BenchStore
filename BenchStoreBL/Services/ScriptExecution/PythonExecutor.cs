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

        public async Task<string> RunScript(string command, IEnumerable<OptionArgument> optionArguments)
        {
            string pythonPath = _tableGeneratorOptions.PythonPath;

            string combinedOptionArguments = CombineOptionArguments(optionArguments);

            ProcessStartInfo startProcess = new ProcessStartInfo();
            startProcess.FileName = pythonPath;
            startProcess.Arguments = $"\"{command}\" {combinedOptionArguments}";
            startProcess.UseShellExecute = false;
            startProcess.CreateNoWindow = true;
            startProcess.RedirectStandardOutput = true;
            startProcess.RedirectStandardError = true;

            using (Process? process = Process.Start(startProcess))
            {
                if (process == null)
                {
                    throw new InvalidOperationException($"Could not start process: '{pythonPath}' with command: '{command}' and args: '{combinedOptionArguments}'");
                }

                using (StreamReader stdoutReader = process.StandardOutput)
                {
                    string stderr = await process.StandardError.ReadToEndAsync();
                    string result = await stdoutReader.ReadToEndAsync();
                    return result;
                }
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

