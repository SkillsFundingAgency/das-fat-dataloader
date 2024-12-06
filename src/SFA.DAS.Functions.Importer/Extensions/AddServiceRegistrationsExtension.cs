using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Functions.Importer.Application.Services;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;
using SFA.DAS.Functions.Importer.Infrastructure;
using System;

namespace SFA.DAS.Functions.Importer.Extensions;

public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .RegisterServices()
            .AddHttpClient()
            .RegisterHttpClients()
            .BindConfiguration(configuration);

        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();
        return services;
    }

    private static IServiceCollection RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IImportDataService, ImportDataService>(options => options.Timeout = TimeSpan.FromMinutes(30));
        return services;
    }

    private static IServiceCollection BindConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ImporterConfiguration>(configuration.GetSection("Importer"));
        services.AddSingleton(new ImporterEnvironment(configuration["EnvironmentName"]!));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ImporterConfiguration>>()!.Value);
        
        return services;
    }
}
