using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BenchStoreBL.Models
{
    public class ResultEntry : IModel
    {
        public int ID { get; set; }

        [DisplayName("Owner Name")]
        [Required]
        public string? OwnerName { get; set; }

        [Display(Name = "Description", Description = "(Optional)")]
        public string? Description { get; set; }

        public DateTime LastAccessTime { get; set; }

        public Result? Result { get; set; }

        public IEnumerable<Label>? Labels { get; set; }

        [Display(Name = "Result Subdirectory Name")]
        public string ResultSubdirectoryName { get; set; } = string.Empty;

        [Display(Name = "Result File Name")]
        public string ResultFileName { get; set; } = string.Empty;

        [Display(Name = "Logfiles Name")]
        public string? LogFilesName { get; set; }
    }
}

