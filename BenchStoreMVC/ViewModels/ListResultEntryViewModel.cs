using BenchStoreBL.Models;

namespace BenchStoreMVC.ViewModels
{
    public class ListResultEntryViewModel
    {
        public ResultEntry? ResultEntry { get; set; }
        public Result? Result { get; set; }
        public IEnumerable<Label>? Labels { get; set; }
        public SelectResultEntryViewModel? SelectResult { get; set; }
    }
}

