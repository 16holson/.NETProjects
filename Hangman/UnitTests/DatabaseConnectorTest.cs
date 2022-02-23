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
            string filepath = "../../Hangman/Server/assets";
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
    }
}