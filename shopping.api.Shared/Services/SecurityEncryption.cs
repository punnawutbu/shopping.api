using System.Text;
using System;
using BC = BCrypt.Net.BCrypt;
using shopping.api.Shared.Util;
using shopping.api.Shared.Models;
using System.Security.Cryptography;

namespace shopping.api.Shared.Services
{
    public class SecurityEncryption : ISecurityEncryption
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly string _keyString;
        private readonly string _envString;
        public SecurityEncryption() { }
        public SecurityEncryption(string keyString)
        {
            _keyString = keyString;
        }
        public SecurityEncryption(string privateKey, string publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }
        public SecurityEncryption(CbcModel model)
        {
            _keyString = model.SecretKey;
            _envString = model.Env;
        }

        public string BCryptComputeHash(string txt, bool hasSalt = false)
        {
            var salt = BC.GenerateSalt(10);
            return hasSalt ? BC.HashPassword(txt, salt) : BC.HashPassword(txt);
        }

        public bool BCryptVerify(string txt, string hash)
        {
            return BC.Verify(txt, hash);
        }

        public string ComputeHash(string strText, string key)
        {
            var hashProvider = new HMACSHA512
            {
                Key = Encoding.UTF8.GetBytes(key)
            };
            return Convert.ToBase64String(hashProvider.ComputeHash(Encoding.UTF8.GetBytes(strText)));
        }

        public string DecryptString(string cipherText)
        {
            byte[] inputArray = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_keyString);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = aes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        public string EncryptString(string plainText)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(plainText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_keyString);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = aes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string EncryptCBC(string plainText)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(plainText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ImportKey.SecretKey(_keyString));
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = Encoding.UTF8.GetBytes(ImportKey.InitVector(_envString));

            ICryptoTransform cTransform = aes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);

            return Convert.ToBase64String(resultArray);
        }

        public string DecryptCBC(string cipherText)
        {
            byte[] inputArray = Convert.FromBase64String(cipherText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ImportKey.SecretKey(_keyString));
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = Encoding.UTF8.GetBytes(ImportKey.InitVector(_envString));
            ICryptoTransform cTransform = aes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            aes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }


        public string RSADecrypt(string txt, bool DoOAEPPadding)
        {
            return RsaUtil.RSADecrypt(txt, _privateKey, DoOAEPPadding);
        }

        public string RSAEncrypt(string txt, bool DoOAEPPadding)
        {
            return RsaUtil.RSAEncrypt(txt, _publicKey, DoOAEPPadding);
        }

        public string RSADecryptWithReadPathKey(string txt, bool DoOAEPPadding)
        {
            string pkPath = _privateKey;
            return RsaUtil.RSADecryptWithFile(txt, pkPath, DoOAEPPadding);
        }
        public string RSADecrypt(string txt)
        {
            return RsaUtil.RSADecrypt(txt, Key.Pk);
        }

        public string ComputeHashWithSalt(string plainText, string saltKey)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_keyString));

            var salt = _GenerateSalt(plainText, saltKey);

            // Combine data, shareKey, and salt
            var combinedData = $"{plainText}{_keyString}{salt}";

            // Compute hash
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(combinedData));
            return Convert.ToBase64String(hash);
        }

        #region Private Functions

        private string _GenerateSalt(string plainText, string saltKey)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_keyString));

            string strData = plainText.Length switch
            {
                < 6 => plainText,
                >= 6 and < 12 => plainText[..6],
                _ => plainText.Substring(plainText.Length / 2, 6)
            };

            var saltBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{strData}{saltKey}"));

            return Convert.ToBase64String(saltBytes);
        }

        #endregion
    }
}