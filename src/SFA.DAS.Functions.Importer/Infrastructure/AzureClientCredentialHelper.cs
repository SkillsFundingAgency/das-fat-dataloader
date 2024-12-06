using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.Infrastructure;

public class AzureClientCredentialHelper : IAzureClientCredentialHelper
{   
    public async Task<string> GetAccessTokenAsync(string identifier)
    {
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(identifier);
     
        return accessToken;
    }
}