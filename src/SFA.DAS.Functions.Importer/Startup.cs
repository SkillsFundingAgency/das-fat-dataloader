using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Functions.Importer;
using SFA.DAS.Functions.Importer.Application.Services;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;
using SFA.DAS.Functions.Importer.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Functions.Importer
{
    
    public class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            
            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration =  serviceProvider.GetService<IConfiguration>();
            
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("local.settings.json", true)
                .AddJsonFile("local.settings.Development.json", true)
#endif
                .AddEnvironmentVariables();

            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }
            
            _configuration = config.Build();
            builder.Services.Configure<ImporterConfiguration>(_configuration.GetSection("Importer"));
            builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ImporterConfiguration>>().Value);

            builder.Services.AddSingleton(new ImporterEnvironment(configuration["EnvironmentName"]));

            builder.Services.AddHttpClient<IImportDataService, ImportDataService>(options=>options.Timeout = TimeSpan.FromMinutes(30));
            builder.Services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();
            
            builder.Services.BuildServiceProvider();
        }
    }
}