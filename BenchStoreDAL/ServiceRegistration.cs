using BenchStoreDAL.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BenchStoreDAL
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterDALServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            string connectionStringKey = "BenchStoreContext";
            string connectionString = configuration.GetConnectionString(connectionStringKey)
                    ?? throw new InvalidOperationException($"Connection string '{connectionStringKey}' not found.");

            serviceCollection.AddDbContext<BenchStoreContext>(options =>
                        options.UseNpgsql(connectionString));

            return serviceCollection;
        }
    }
}

