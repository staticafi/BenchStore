using BenchStoreDAL.Entities;

using Microsoft.EntityFrameworkCore;

namespace BenchStoreDAL.Data
{
    public class BenchStoreContext : DbContext
    {
        public BenchStoreContext(DbContextOptions<BenchStoreContext> options)
            : base(options) { }

        public DbSet<ResultEntry> ResultEntry { get; set; }

        public DbSet<Result> Result { get; set; }

        public DbSet<Label> Label { get; set; }
    }
}

