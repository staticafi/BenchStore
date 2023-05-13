using System.ComponentModel.DataAnnotations;

using BenchStoreBL.Models;
using BenchStoreBL.Services.ResultEntries;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace BenchStoreMVC.ViewModels
{
    public class FilterResultEntriesViewModel
    {
        public SelectList? Tools { get; set; }

        [Display(Name = "Labels", Prompt = "Labels")]
        public string? LabelsInput { get; set; }
        public IEnumerable<Label>? Labels { get; set; }
        public OrderResultEntryBy OrderResultEntryBy { get; set; } = OrderResultEntryBy.Date;
        public ResultEntriesFilter? ResultEntriesFilter { get; set; }
    }
}

