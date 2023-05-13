namespace BenchStoreBL.Services.ScriptExecution
{
    public interface ITableGeneratorExecutor
    {
        Task<string> ExecuteTableGenerator(IEnumerable<string> resultFilePaths, IEnumerable<string>? logFilesPaths = null);
    }
}

