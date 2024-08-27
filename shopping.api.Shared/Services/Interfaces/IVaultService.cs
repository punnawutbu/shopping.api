using System.Threading.Tasks;

namespace shopping.api.Shared.Services
{
    public interface IVaultService
    {
        Task<string> GetCredential(string secretPath);
        Task<string> GetCredential(string secretPath, string token);

    }
}