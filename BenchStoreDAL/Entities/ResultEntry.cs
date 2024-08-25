﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchStoreDAL.Entities
{
    public class ResultEntry : IEntity
    {
        [Key]
        public int ID { get; set; }

        public string? OwnerName { get; set; }

        public string? Description { get; set; }

        public DateTime LastAccessTime { get; set; }

        public ICollection<Label> Labels { get; set; } = new List<Label>();

        public Result? Result { get; set; }

        [ForeignKey(nameof(Result))]
        public int ResultID { get; set; }

        public required string ResultSubdirectoryName { get; set; }

        public string? ResultFileName { get; set; }

        public string? LogFilesName { get; set; }
    }
}
