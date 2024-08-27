using System;
using System.Security.Cryptography;
using System.Text;

namespace shopping.api.Shared.Util
{
    public static class RsaUtil
    {
        public static string RSADecrypt(string txt, string privateKey, bool DoOAEPPadding)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(privateKey);

            byte[] byteEntry = Convert.FromBase64String(txt);
            byte[] byteText = rsa.Decrypt(byteEntry, DoOAEPPadding);

            return Encoding.UTF8.GetString(byteText);
        }
        public static string RSAEncrypt(string txt, string publicKey, bool DoOAEPPadding)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(publicKey);

            byte[] byteText = Encoding.UTF8.GetBytes(txt);
            byte[] byteEntry = rsa.Encrypt(byteText, DoOAEPPadding);

            return Convert.ToBase64String(byteEntry);
        }
        public static string RSADecryptWithFile(string txt, string pkPath, bool DoOAEPPadding)
        {
            var rsa = ImportKey.GetPrivateKeyFromPemFile(pkPath);
            byte[] byteText = rsa.Decrypt(Convert.FromBase64String(txt), DoOAEPPadding);
            return Encoding.UTF8.GetString(byteText);
        }
        public static string RSADecrypt(string txt, string privateKey)
        {
            RSACryptoServiceProvider rsa = ImportKey.GetPrivateKey(privateKey);

            byte[] byteEntry = Convert.FromBase64String(txt);
            byte[] byteText = rsa.Decrypt(byteEntry, false);

            return Encoding.UTF8.GetString(byteText);
        }
    }
}