
namespace shopping.api.Shared.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityEncryption _securityEncryption;
        public SecurityService(ISecurityEncryption securityEncryption, object hashKey)
        {
            _securityEncryption = securityEncryption;

        }
        public string BcryptPassword(string pass)
        {
            return _securityEncryption.BCryptComputeHash(pass);
        }

        public bool PasswordVerify(string pass, string hash)
        {
            return _securityEncryption.BCryptVerify(pass, hash);
        }
    }
}