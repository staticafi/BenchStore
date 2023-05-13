using BenchStoreBL.Models;

namespace BenchStoreBL.Services.Labels
{
    public interface ILabelsService
    {
        Task<List<Label>> GetLabels();
        Task<List<Label>> GetResultEntryLabels(int resultEntryID);
        Task<Label?> GetLabelByID(int id);
        Task<Label?> GetLabelByName(string name);
        Task<int> CreateLabel(Label label);
        Task EditLabel(Label label);
    }
}

