using System.ComponentModel.DataAnnotations;

namespace BenchStoreBL.Models
{
    public class Label : IModel
    {
        public int ID { get; set; }

        [Required]
        public required string Name { get; set; }

        [RegularExpression(@"^#(?:[0-9a-fA-F]{3}){1,2}$")]
        public string? Color { get; set; }
    }
}

