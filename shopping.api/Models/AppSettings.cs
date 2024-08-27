
using shopping.api.Shared.Models;

namespace shopping.api.Models
{
    public class AppSettings
    {
        public string VaultHost { get; set; }
        public string Shopping { get; set; }
        public CredentialSetting CredentialSetting { get; set; }
    }
    public class CredentialSetting
    {
        public Certificate Certificate { get; set; }
        public string ShoppingConnectionString { get; set; }
        public string MfaAuthConnectionString { get; set; }
        public string ApplicationConnectionString { get; set; }
        public string MongoDbConnectionString { get; set; }
        public bool SslMode { get; set; }
        public string HashKey { get; set; }
        public string PublicKey { get; set; }
        public string SecertKey { get; set; }

    }
}