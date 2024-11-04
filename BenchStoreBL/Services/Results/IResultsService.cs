using BenchStoreBL.Models;

namespace BenchStoreBL.Services.Results
{
    public interface IResultsService
    {
        Task<Result?> GetResultByID(int id);

        Task<Result?> GetResultByResultEntryID(int resultEntryID);

        string GetResultName(Result result);

        string GetLogFilesName(Result result);
    }
}

