using System.Text;
using System.Security.Cryptography;

namespace NetDeviceManager.Lib.Utils
{
    public class DesAlgorithm
    {
        public static string Encrypt(string input, string key)
        {
            using (TripleDES des = TripleDES.Create())
            {
                des.Key = MD5.HashData(Encoding.UTF8.GetBytes(CookKey(key)));
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                var bytesToEncrypt = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            }
        }

        public static string Decrypt(string input, string key)
        {
            using (TripleDES des = TripleDES.Create())
            {
                des.Key = MD5.HashData(Encoding.UTF8.GetBytes(CookKey(key)));
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                var bytesToDecrypt = Convert.FromBase64String(input);
                return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            }
        }

        private static string CookKey(string key)
        {
            return $"salt|{Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(key)))}|peper";
        }
    }
}