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
            string filepath = "../../../../Hangman/Server/assets/";
            string filename = "HangmanDB.accdb";
            // Create new DatabaseConnector object
            DatabaseConnector dbConnectorObj = new DatabaseConnector(filepath, filename); 
            // Test instantiation
            Assert.IsNotNull(dbConnectorObj);
            Assert.IsInstanceOfType(dbConnectorObj, typeof(Hangman.Server.CustomClasses.DatabaseConnector));
        }

        [TestMethod]
        public void TestOpenConnection()
        {
            // Arrange
            string filepath = "../../../../Hangman/Server/assets/";
            string filename = "HangmanDB.accdb";
            // Create new DatabaseConnector object
            DatabaseConnector dbConnectorObj = new DatabaseConnector(filepath, filename);
            dbConnectorObj.Connect();
        }

        [TestMethod]
        public void TestProcessFile()
        {
        }
    }
}