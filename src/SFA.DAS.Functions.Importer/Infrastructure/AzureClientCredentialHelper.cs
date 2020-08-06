using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.Infrastructure
{
    public class AzureClientCredentialHelper : IAzureClientCredentialHelper
    {   
        public async Task<string> GetAccessTokenAsync(string identifier)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(identifier);
         
            return accessToken;
        }
    }
}