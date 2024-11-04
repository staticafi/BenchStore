using BenchStoreBL.Options;
using BenchStoreBL.Services.Labels;
using BenchStoreBL.Services.ResultEntries;
using BenchStoreBL.Services.Results;
using BenchStoreBL.Services.ResultStoring;
using BenchStoreBL.Services.ScriptExecution;
using BenchStoreBL.Services.XMLElementParsing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BenchStoreBL
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterBLConfig(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<StorageOptions>(
                configuration.GetSection(StorageOptions.Storage));

            serviceCollection.Configure<TableGeneratorOptions>(
                configuration.GetSection(TableGeneratorOptions.TableGenerator));

            return serviceCollection;
        }

        public static IServiceCollection RegisterBLServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IResultEntriesService, ResultEntriesService>();
            serviceCollection.AddScoped<IResultsService, ResultsService>();
            serviceCollection.AddScoped<ILabelsService, LabelsService>();
            serviceCollection.AddScoped<IXMLElementParser, XMLElementParser>();
            serviceCollection.AddScoped<IFileStorage, ResultFileStorage>();
            serviceCollection.AddScoped<IScriptExecutor, PythonExecutor>();
            serviceCollection.AddScoped<ITableGeneratorExecutor, TableGeneratorExecutor>();

            return serviceCollection;
        }
    }
}

