using Hangman.Server.CustomClasses;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.IO;

namespace Hangman.Server
{
    public class NotificationHub : Hub {
        //private static string currentDir = Directory.GetCurrentDirectory();
        private static string DATABASE_FILE_PATH = Directory.GetCurrentDirectory() + @"\assets";
        private const string DATABASE_FILE_NAME = "HangmanDB.accdb";
        private DatabaseConnector dbConnector = new DatabaseConnector(DATABASE_FILE_PATH, DATABASE_FILE_NAME);


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
        /// Checks authenticates the user in the db and returns a message with the authentication status
        /// </summary>
        /// <param name="user"></param>
        /// <param name="salt"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Authenticate(string user, string password)
        {
            // If user doesn't exist, or password is wrong, return false authentication status
            string dbPassword = "someHashedPassword";
            bool isAuthenticated = dbConnector.ValidateUser(user, password);
            await Clients.All.SendAsync("LoginConfirmation", user, password, isAuthenticated);
        }

        /// <summary>
        /// Takes information received for a new user and creates a row in the db for the user
        /// Creates salt for hashing a new user's password.
        /// Hashes the password input with the salt
        /// The saltyHashPassword and the salt should be used to create user's the db record
        /// Returns isAuthenticated status, to denote whether the new user record was created successfully
        /// </summary>
        /// <param name="user"></param>
        /// <param name="salt"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public async Task NewAccount(string user, string password) {


            bool isAuthenticated = false;

            //Checks if user already exists within the database
            if ((dbConnector.FindUser(user)).Username == "UserNotFound") {

                var salty = SaltyHash.GenerateSalt();
                var hashedPassword = SaltyHash.ComputeSha256Hash(Encoding.UTF8.GetBytes(password), salty);
                string saltyHashPassword = BitConverter.ToString(hashedPassword);
                

                try {
                    //Inserts the new user into the database
                    if (dbConnector.InsertUser(user, saltyHashPassword, SaltyHash.ConvertToString(salty)) > 0) {
                        isAuthenticated = true;
                    }
                }
                catch (Exception e) {
                    isAuthenticated = false;
                }

                //Responds to the server if the user was successful in being inserted into the database
                await Clients.All.SendAsync("NewAccountConfirmation", user, saltyHashPassword, isAuthenticated);


            } else {

                //Responds to the server if the user already exists in the database
                await Clients.All.SendAsync("NewAccountConfirmation", user, password, isAuthenticated);
            }

        }
    }
}