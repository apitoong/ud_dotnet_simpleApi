using System.Security.Cryptography;
using System.Text;

namespace simpleApi.Helpers;

public class Helper
{
    private static readonly string EncryptionKey = "YourSecretEncryptionKey";

    public static string StringToByte(string data)
    {
        if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(EncryptionKey))
            throw new ArgumentException("Data dan kunci tidak boleh kosong.");

        try
        {
            // Menggunakan SHA-256 untuk menghasilkan kunci yang sesuai dengan panjang yang dibutuhkan oleh AES-256.
            using var sha256 = new SHA256CryptoServiceProvider();
            var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));

            using var aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;

            // IV (Initialization Vector) digunakan untuk meningkatkan keamanan enkripsi.
            // IV harus unik dan tidak dirahasiakan.
            aesAlg.IV = new byte[aesAlg.BlockSize / 8];

            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Gagal mengenkripsi data. Error: {ex.Message}");
        }
    }


    public static bool StringToBoolean(string value)
    {
        if (bool.TryParse(value, out var result))
            return result;

        return false;
    }

    public static int StringToInteger(string value)
    {
        if (int.TryParse(value, out var result))
            return result;

        return 0;
    }
}