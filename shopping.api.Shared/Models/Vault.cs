using Flurl.Http;

namespace shopping.api.Shared.Models
{
    public class Vault
    {
        public class VaultConfig
        {
            public IFlurlClient Url { get; set; }
            public string Token { get; set; }
        }
        public class VaultResponse
        {
            public VaultData Data { get; set; }
        }
        public class VaultData
        {
            public VaultCredential Data { get; set; }
        }
        public class VaultCredential
        {
            public string Credential { get; set; }
        }

    }
}