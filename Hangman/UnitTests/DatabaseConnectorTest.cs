using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hangman.Server.CustomClasses;

namespace UnitTests
{
    [TestClass]
    public class DatabaseConnectorTest
    {
        [TestMethod]
        public void TestInstantiation()
        {
            // Arrange
            string filepath = "../../../../Hangman/Server/assets";
            string filename = "HangmanDB.accdb";
            // Create new DatabaseConnector object
            DatabaseConnector dbConnector = new DatabaseConnector(filepath, filename);
            // Assert
            Assert.IsNotNull(dbConnector);
            Assert.IsInstanceOfType(dbConnector, typeof(DatabaseConnector));
        }

        [TestMethod]
        public void TestInsertUser()
        {
            string filepath = "../../../../Hangman/Server/assets";
            string filename = "HangmanDB.accdb";
            DatabaseConnector dbConnector = new DatabaseConnector(filepath, filename);
            byte[] salt = SaltyHash.GenerateSalt();
            byte[] hashedPass = SaltyHash.ComputeSha256Hash(SaltyHash.ConvertToBytes("password"), salt);
            string saltString = SaltyHash.ConvertToString(salt);
            string hashedPassString = SaltyHash.ConvertToString(hashedPass);
            // Try
            Assert.AreEqual(1, dbConnector.InsertUser("testingtester" + saltString, hashedPassString, saltString));
        }

        [TestMethod]
        public void TestFindUser()
        {
            string filepath = "../../../../Hangman/Server/assets";
            string filename = "HangmanDB.accdb";
            DatabaseConnector dbConnector = new DatabaseConnector(filepath, filename);
            User notFoundUser = dbConnector.FindUser("NonExistantUser");
            User user = dbConnector.FindUser("useruseruser");
            // Case: user not found, should return user with username UserNotFound 
            Assert.IsNotNull(notFoundUser);
            Assert.IsInstanceOfType(notFoundUser, typeof(User));
            Assert.AreEqual("UserNotFound", notFoundUser.Username);
            Assert.AreEqual("NoPassword", notFoundUser.HashedPass);
            // Case: user is found
            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(user, typeof(User));
            Assert.AreEqual("useruseruser", user.Username);
            Assert.AreEqual("passhashsalt", user.HashedPass);
        }
    }
}