namespace BenchStoreBL.Services.ScriptExecution
{
    public interface ITableGeneratorExecutor
    {
        Task<string> ExecuteTableGenerator(string hostUrl, IEnumerable<string> resultFilePaths, IEnumerable<string>? logFilesPaths = null);
    }
}

