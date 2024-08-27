using System.IO;
using System.Net;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace shopping.api.Shared.Util
{
    public static class ImportKey
    {
        public static RSACryptoServiceProvider GetPublicKeyFromPemFile(string filePath)
        {
            using (TextReader publicKeyTextReader = new StringReader(File.ReadAllText(_SecretKey(filePath))))
            {
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)new PemReader(publicKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParam);

                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
                csp.ImportParameters(rsaParams);
                return csp;
            }
        }

        public static RSACryptoServiceProvider GetPrivateKeyFromPemFile(string filePath)
        {
            using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(_SecretKey(filePath))))
            {
                AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
                
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
                csp.ImportParameters(rsaParams);
                return csp;
            }
        }
        public static RSACryptoServiceProvider GetPrivateKey(string pk)
        {
            using (TextReader privateKeyTextReader = new StringReader(pk))
            {
                AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
                
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(4096);
                csp.ImportParameters(rsaParams);
                return csp;
            }
        }

        private static string _SecretKey(string pathSecretKey)
        {
            var client = new WebClient();
            var fullPath = Path.Combine(@"secretKey.pem");
            client.DownloadFile(pathSecretKey, fullPath);
            client.Dispose();
            return fullPath;
        }

        public static string SecretKey(string pathSecretKey)
        {
            var client = new WebClient();
            var fullPath = Path.Combine(@"secretKey.txt");
            client.DownloadFile(pathSecretKey, fullPath);
            client.Dispose();
            return File.ReadAllText(fullPath);
        }

        public static string InitVector(string env)
        {
            switch (env)
            {
                case "Development":
                    return "6@al5td%98G#fBrs";
                case "Staging":
                    return "XmtfnR7H2*G6564J";
                case "Production":
                    return "&8Xh4T6k1yO9Ol*m";
                default :
                    return "";
            }
        }
    }
}