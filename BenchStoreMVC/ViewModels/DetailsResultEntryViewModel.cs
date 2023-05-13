using BenchStoreBL.Models;

namespace BenchStoreMVC.ViewModels
{
    public class DetailsResultEntryViewModel
    {
        public required ResultEntry ResultEntry { get; set; }
        public required IEnumerable<Label> Labels { get; set; }
    }
}

