using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.Application.Services
{
    public class ImportDataService : IImportDataService
    {
        private readonly HttpClient _client;
        private readonly IAzureClientCredentialHelper _azureClientCredentialHelper;
        private readonly ImporterEnvironment _importerEnvironment;
        private readonly ImporterConfiguration _configuration;

        public ImportDataService(  HttpClient client, IOptions<ImporterConfiguration> configuration,
            IAzureClientCredentialHelper azureClientCredentialHelper, ImporterEnvironment importerEnvironment)
        {
            _client = client;
            _azureClientCredentialHelper = azureClientCredentialHelper;
            _importerEnvironment = importerEnvironment;
            _configuration = configuration.Value;
        }

        public void Import()
        {
            var taskList = new List<Task>();
            AddVersionHeader("1.0");
            foreach (var dataLoadOperation in _configuration.DataLoaderBaseUrlsAndIdentifierUris.Split(","))
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

            Task.WhenAll(taskList.ToArray());
        }
        
        private void AddVersionHeader(string requestVersion)
        {
            _client.DefaultRequestHeaders.Add("X-Version", requestVersion);
        }
    }
}