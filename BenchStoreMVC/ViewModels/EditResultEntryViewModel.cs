using System.ComponentModel.DataAnnotations;

using BenchStoreBL.Models;

namespace BenchStoreMVC.ViewModels
{
    public class EditResultEntryViewModel
    {
        public required ResultEntry ResultEntry { get; set; }
        public required Result Result { get; set; }
        public IEnumerable<Label>? Labels { get; set; }

        [Display(Name = "Labels", Description = "(Optional, separated by \",\")")]
        public string? LabelsInput { get; set; }
    }
}

