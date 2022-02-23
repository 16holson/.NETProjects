using System.Security.Cryptography;
using System.Text;

namespace Hangman.Server.CustomClasses
{
    public static class SaltyHash
    {
        private const int SaltSize = 32;
        /// <summary>
        /// Generates a random integer for the salt
        /// </summary>
        /// <returns>Salted integer as byte[]</returns>
        public static byte[] GenerateSalt()
        {
            using(var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SaltSize];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        /// <summary>
        /// Hashes the data with the salt provided
        /// </summary>
        /// <param name="data"></param>
        /// <param name="salt"></param>
        /// <returns>Hashed and salted data as a byte[]</returns>
        public static byte[] ComputeSha256Hash(byte[] data, byte[] salt)
        {
            using (var hash = new HMACSHA256(salt))
            {
                return hash.ComputeHash(data);
            }
        }

        /// <summary>
        /// Accepts a byte array and returns a string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ConvertToString(byte[] data)
        {
            return BitConverter.ToString(data);
        }

        /// <summary>
        /// Accepts a string and returns a byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

    }
}
