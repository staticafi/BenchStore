using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BenchStoreBL.Models
{
    public class Result : IModel
    {
        public int ID { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [DisplayName("Run Definition Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [DisplayName("Benchmark Name")]
        public string? BenchmarkName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [DisplayName("Display Name")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        public string? Block { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        public string? Tool { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [DisplayName("Tool-Info Module")]
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
        [Display(Name = "MemLimit", Description = "(Bytes)")]
        public long MemLimit { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [Display(Name = "TimeLimit", Description = "(seconds)")]
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
    }
}

