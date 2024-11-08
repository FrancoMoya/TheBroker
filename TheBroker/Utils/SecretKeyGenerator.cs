using System.Security.Cryptography;

namespace TheBroker.Utils
{
    public class SecretKeyGenerator
    {
        public static string GenerateKey()
        {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[32]; // 32 bytes = 256 bits
            random.GetBytes(key);  // Llena el arreglo con bytes aleatorios
            return Convert.ToBase64String(key); // Convierte el arreglo a Base64
        }
    }
}
