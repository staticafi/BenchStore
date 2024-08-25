using System.ComponentModel.DataAnnotations;

namespace BenchStoreBL.Services.ResultEntries
{
    public class ResultEntriesFilter
    {
        [Display(Name = "Benchmark Name", Prompt = "Benchmark Name")]
        public string? BenchmarkName { get; set; }

        [Display(Name = "Run Definition Name", Prompt = "Run Definition Name")]
        public string? Name { get; set; }

        [Display(Name = "Tool", Prompt = "Tool")]
        public string? Tool { get; set; }

        [Display(Name = "Owner Name", Prompt = "Owner Name")]
        public string? OwnerName { get; set; }

        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }

        public IEnumerable<string>? LabelNames { get; set; }
    }
}

