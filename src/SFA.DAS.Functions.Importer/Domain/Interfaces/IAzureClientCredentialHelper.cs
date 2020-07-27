using System.Threading.Tasks;

namespace SFA.DAS.Functions.Importer.Domain.Interfaces
{
    public interface IAzureClientCredentialHelper
    {
        Task<string> GetAccessTokenAsync(string identifier);
    }
}