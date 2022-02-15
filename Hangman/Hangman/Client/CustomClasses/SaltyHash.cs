using System.Security.Cryptography;
using System.Text;

namespace Hangman.Client.CustomClasses
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

    }
}
