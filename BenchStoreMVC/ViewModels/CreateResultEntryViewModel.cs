using System.ComponentModel.DataAnnotations;

using BenchStoreBL.Models;

namespace BenchStoreMVC.ViewModels
{
    public class CreateResultEntryViewModel
    {
        public required ResultEntry ResultEntry { get; set; }

        public required Result Result { get; set; }

        public IEnumerable<Label> Labels { get; set; } = new List<Label>();

        [Display(Name = "Labels", Description = "(Optional, separated by \",\")")]
        public string? LabelsInput { get; set; }

        public required string ResultFileTempPath { get; set; }

        public string? ResultLogsTempPath { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public required string FileNamePrefix { get; set; }
    }
}

