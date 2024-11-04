using BenchStoreBL.Models;

namespace BenchStoreMVC.ViewModels
{
    public class ListResultEntryViewModel
    {
        public required ResultEntry ResultEntry { get; set; }

        public required Result Result { get; set; }

        public IEnumerable<Label> Labels { get; set; } = new List<Label>();

        public required SelectResultEntryViewModel SelectResult { get; set; }
    }
}

