using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using static shopping.api.Shared.Models.Vault;

namespace shopping.api.Shared.Services
{
    public class VaultService : IVaultService
    {
        private readonly VaultConfig _config;
        public VaultService(VaultConfig config)
        {
            _config = config;
        }
        public async Task<string> GetCredential(string secretPath)
        {
            var resp = await _config.Url
            .WithHeader("X-Vault-Token", _config.Token)
            .Request(secretPath)
            .GetJsonAsync<VaultResponse>();

            return resp.Data.Data.Credential;
        }

        public async Task<string> GetCredential(string secretPath, string token)
        {
            var resp = await _config.Url
            .WithHeader("X-Vault-Token", token)
            .Request(secretPath)
            .GetJsonAsync<VaultResponse>();

            return resp.Data.Data.Credential;
        }
    }

}