
namespace shopping.api.Shared.Services
{
    public interface ISecurityService
    {
        string BcryptPassword(string pass);

        bool PasswordVerify(string pass, string hash);
    }
}