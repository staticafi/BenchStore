namespace BenchStoreBL.Services.ScriptExecution
{
    public interface IScriptExecutor
    {
        public Task<(string, string)> RunScript(string command, IEnumerable<OptionArgument> arguments);
    }
}

