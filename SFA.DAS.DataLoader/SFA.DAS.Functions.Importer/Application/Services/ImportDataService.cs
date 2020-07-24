using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            if (!_importerEnvironment.EnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                var token = _azureClientCredentialHelper.GetAccessTokenAsync().Result;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);    
            }

            foreach (var url in _configuration.Urls.Split(","))
            {
                _client.PostAsync($"{url}ops/dataload", null).ConfigureAwait(false);
            };
        }
    }
}