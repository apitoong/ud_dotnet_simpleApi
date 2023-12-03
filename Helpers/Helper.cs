using System.Security.Cryptography;
using System.Text;

namespace simpleApi.Helpers;

public class Helper
{
    private static readonly string EncryptionKey = "YourSecretEncryptionKey";

    public static string EncryptData(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.IV = new byte[16];

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] encryptedBytes;

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                encryptedBytes = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}