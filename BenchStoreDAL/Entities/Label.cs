using System.ComponentModel.DataAnnotations;

namespace BenchStoreDAL.Entities
{
    public class Label : IEntity
    {
        [Key]
        public int ID { get; set; }

        public string? Name { get; set; }

        public string? Color { get; set; }

        public ICollection<ResultEntry> ResultEntries { get; set; } = new List<ResultEntry>();
    }
}
