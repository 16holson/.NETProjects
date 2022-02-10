using Hangman.Server.CustomClasses;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace Hangman.Server
{
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Receives two strings from client. Returns the same strings received.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Receives user name and password
        /// Returns the user, the salt (in string format) and the hashed and salted password (in string format)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="salt"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Authenticate(string user, string salt, string dbPassword)
        {
            // TODO: Check db for user. If user exists, return salt and hashed password
            await Clients.All.SendAsync("LoginConfirmation", user, salt, dbPassword );
        }

        /// <summary>
        /// Takes information received for a new user and creates a row in the db for the user
        /// Creates salt for hashing a new user's password.
        /// Hashes the password input with the salt
        /// The saltyHashPassword and the salt should be used to create user's the db record
        /// </summary>
        /// <param name="user"></param>
        /// <param name="salt"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public async Task NewAccount(string user, string salt, string unhashedPassword)
        {
            // TODO: Check db for user. If user exists, return salt and hashed password
            var salty = SaltyHash.GenerateSalt();
            var hashedPassword = SaltyHash.ComputeSha256Hash(Encoding.UTF8.GetBytes(unhashedPassword), salty);
            salt = BitConverter.ToString(salty);
            string saltyHashPassword = BitConverter.ToString(hashedPassword);
            await Clients.All.SendAsync("NewAccountConfirmation", user, salt, saltyHashPassword);
        }
    }
}
