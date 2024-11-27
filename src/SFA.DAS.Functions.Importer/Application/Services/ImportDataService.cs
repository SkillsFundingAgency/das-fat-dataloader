using Microsoft.Extensions.Options;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;
using System.Net.Http.Headers;

namespace SFA.DAS.Functions.Importer.Application.Services;

public class ImportDataService(HttpClient _client, IOptions<ImporterConfiguration> _configuration, IAzureClientCredentialHelper _azureClientCredentialHelper, ImporterEnvironment _importerEnvironment) : IImportDataService
{
    public void Import()
    {
        var taskList = new List<Task>();
        AddVersionHeader("1.0");
        foreach (var dataLoadOperation in _configuration.Value.DataLoaderBaseUrlsAndIdentifierUris.Split(","))
        {
            var dataLoadOperationValues = dataLoadOperation.Split("|");
            var url = dataLoadOperationValues[0];
            
            if (!_importerEnvironment.EnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                var identifier = dataLoadOperationValues[1];
                var token = _azureClientCredentialHelper.GetAccessTokenAsync(identifier).Result;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);    
            }
            
            taskList.Add(_client.PostAsync($"{url}ops/dataload", null));
        }

        Task.WhenAll(taskList);
    }
    
    private void AddVersionHeader(string requestVersion)
    {
        _client.DefaultRequestHeaders.Add("X-Version", requestVersion);
    }
}