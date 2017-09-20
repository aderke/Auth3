using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthTestApplication.Models;
using Microsoft.AspNet.Identity;

namespace AuthTestApplication.Managers
{
    public class CustomerUserManager : UserManager<ApplicationUser>
    {
        private List<UserElement> users;
        private RSAParameters _privKey;
        private RSAParameters _pubKey;

        public CustomerUserManager() : base(new CustomerUserStore<ApplicationUser>())
        {
            users = RegisterUserSection.GetConfig().GetUsers();
        }

        public override Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var taskInvoke = Task<ApplicationUser>.Factory.StartNew(() =>
            {
                var user = users.FirstOrDefault(u => u.Name == userName);

                if (user != null && userName == user.Name && CalculateHash(password) == user.Password)
                {
                    return new ApplicationUser
                    {
                        Id = user.Name,
                        UserName = user.Name,
                        Role = (ApplicationRole) Enum.Parse(typeof(ApplicationRole), user.Role, true)
                    };
                }

                return null;
            });

            return taskInvoke;
        }

        public EncryptModel EncryptDecrypt(string text)
        {
            // generate 32bit random key
            byte[] key = GenerateAes256Key();

            // encrypt user text with AES256
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = EncryptAes256(textBytes, key);
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            // encrypt key with RSA2048
            string encryptedKey = EncryptRsa2048(key);
            // decrypt key with RSA2048
            byte[] decryptedKey = DecryptRsa2048(encryptedKey);

            // decrypt user text with AES256
            byte[] decryptedBytes = DecryptAes256(encryptedBytes, decryptedKey);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

            return new EncryptModel { Encrypted = encryptedText, Decrypted = decryptedText };
        }

        #region Helpers
        private string CalculateHash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        private byte[] GenerateAes256Key(int keyBytes = 32)
        {
            const int Iterations = 300;
            string password = System.Web.Security.Membership.GeneratePassword(20, 5);
            var salt = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };
            var keyGenerator = new Rfc2898DeriveBytes(password, salt, Iterations);
            return keyGenerator.GetBytes(keyBytes);
        }

        private static byte[] EncryptAes256(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
           
            byte[] saltBytes = { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] DecryptAes256(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
           
            byte[] saltBytes = { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private string EncryptRsa2048(byte[] bytesData)
        {
            var csp = new RSACryptoServiceProvider(2048);
            
            _privKey = csp.ExportParameters(true);
            _pubKey = csp.ExportParameters(false);
            
            var pubKeyString = string.Empty;
            
            var sw = new StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _pubKey);
            pubKeyString = sw.ToString();
           
            var sr = new StringReader(pubKeyString);
            var pxs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            _pubKey = (RSAParameters) pxs.Deserialize(sr);
           
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(_pubKey);
            
            var bytesCypherText = csp.Encrypt(bytesData, false);
            
            return Convert.ToBase64String(bytesCypherText);
        }

        private byte[] DecryptRsa2048(string textToDecrypt)
        {
            var bytesCypherText = Convert.FromBase64String(textToDecrypt);
            
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(_privKey);
            
            return csp.Decrypt(bytesCypherText, false);
        }

        #endregion
    }
}