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

        // [TestMethod]
        // public void TestOpenConnection()
        // {
        //     // Arrange
        //     string filepath = "../../../../Hangman/Server/assets";
        //     string filename = "HangmanDB.accdb";
        //     DatabaseConnector dbConnector = new DatabaseConnector(filepath, filename);
        //     // Try
        //     dbConnector.Connect();
        //     // Assert
        //     // Not sure what to assert. Connect() didn't throw an exception, though. 
        //     dbConnector.CloseDB();  
        // 
        // }

        [TestMethod]
        public void TestInsertUser()
        {
            string filepath = "../../../../Hangman/Server/assets";
            string filename = "HangmanDB.accdb";
            DatabaseConnector dbConnector = new DatabaseConnector(filepath, filename);
            // Try
            Assert.AreEqual(1, dbConnector.InsertUser());
        }
    }
}