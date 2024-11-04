using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BenchStoreDAL.Data
{
    internal class BenchStoreContextFactory : IDesignTimeDbContextFactory<BenchStoreContext>
    {
        public BenchStoreContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}/../BenchStoreMVC")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Development.json", true)
                .AddEnvironmentVariables()
                .Build();

            string connectionStringKey = "BenchStoreContext";
            string connectionString = configuration.GetConnectionString(connectionStringKey)
                    ?? throw new InvalidOperationException($"Connection string '{connectionStringKey}' not found.");

            var builder = new DbContextOptionsBuilder<BenchStoreContext>();
            builder.UseNpgsql(connectionString);

            return new BenchStoreContext(builder.Options);
        }
    }
}
