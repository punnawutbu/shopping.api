
namespace shopping.api.Shared.Services
{
    public interface ISecurityEncryption
    {
        string ComputeHash(string strText, string key);
        string ComputeHashWithSalt(string plainText, string saltKey);
        #region AES
        string EncryptString(string plainText);
        string DecryptString(string cipherText);
        string EncryptCBC(string plainText);
        string DecryptCBC(string cipherText);
        #endregion
        #region BCrypt
        string BCryptComputeHash(string txt, bool hasSalt = false);
        bool BCryptVerify(string txt, string hash);
        #endregion
        #region  RSA
        string RSAEncrypt(string txt, bool DoOAEPPadding);
        string RSADecrypt(string txt, bool DoOAEPPadding);
        string RSADecrypt(string txt);
        string RSADecryptWithReadPathKey(string txt, bool DoOAEPPadding);
        #endregion
    }
}