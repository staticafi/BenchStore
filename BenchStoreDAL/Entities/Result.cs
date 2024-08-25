using System.ComponentModel.DataAnnotations;

namespace BenchStoreDAL.Entities
{
    public class Result : IEntity
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? BenchmarkName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? Block { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public DateTime Date { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? Tool { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? ToolModule { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? Version { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? Options { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public long MemLimit { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public long TimeLimit { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public uint CPUCores { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? Generator { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? Error { get; set; }

        public ResultEntry ResultEntry { get; set; } = null!;
    }
}
