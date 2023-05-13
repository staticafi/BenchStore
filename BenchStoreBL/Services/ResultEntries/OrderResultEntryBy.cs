using System.ComponentModel.DataAnnotations;

namespace BenchStoreBL.Services.ResultEntries
{
    public enum OrderResultEntryBy
    {
        [Display(Name = "Benchmark Name")]
        BenchmarkName,

        [Display(Name = "Run Definition Name")]
        Name,

        [Display(Name = "Owner Name")]
        OwnerName,

        [Display(Name = "Date")]
        Date,

        [Display(Name = "Tool")]
        Tool,
    }
}

