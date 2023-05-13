using BenchStoreBL.Models;

namespace BenchStoreBL.Services.ResultEntries
{
    public interface IResultEntriesService
    {
        Task<List<ResultEntry>> GetResultEntries(OrderResultEntryBy orderResultEntryBy, ResultEntriesFilter? filter = null);
        Task<ResultEntry?> GetResultEntryByID(int id);
        Task<int> CreateResultEntry(ResultEntry resultEntry, Result result);
        Task DeleteResultEntry(int id);
        Task EditResultEntry(ResultEntry resultEntry);
        Task EditResultEntryLabels(int storedResultId, IEnumerable<Label> labels);
    }
}

