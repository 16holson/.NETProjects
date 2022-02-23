using System.Data.Odbc;

namespace Hangman.Server.CustomClasses
{
    public class DatabaseConnector
    {
		/// <summary>
		/// String that gives all data necessary to connect to access db file
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Filepath to get to the folder containing the .accdb file
		/// </summary>
		public string FolderPath { get; set; }
		
		/// <summary>
		/// Name of the .accdb file
		/// </summary>
		public string FileName { get; set; }
		
		/// <summary>
		/// Name of the table you want to search or alter
		/// </summary>
		public string TableName { get; set; } = "Users";

		/// <summary>
		/// Constructor for DatabaseConnector class
		/// Sets the FolderPath, FileName and ConnectionString
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="filename"></param>
		public DatabaseConnector(string filepath, string filename)
        {
			FolderPath = filepath;	
			this.FileName = filename;
			string driverName = "Microsoft Access Driver (*.mdb, *.accdb)";
			var file = Directory.GetFiles(filepath, filename).FirstOrDefault();
			if(File.Exists(file))
            {
				ConnectionString = $"Driver={driverName};DBQ={file};Data Source={TableName}";
			}
		}

		/// <summary>
		/// Check if the username and password combo is valid
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns>
		/// true if username exists and password matches records in db
		/// </returns>
		public bool ValidateUser(string userName, string password) 
		{ 
			using (OdbcConnection odbcConnection = new OdbcConnection(ConnectionString))
            {
				User user = FindUser(userName);
				if (user.Username != "UserNotFound")
                {
					string hash = SaltyHash.ComputeSha256Hash(password, user.Salt).ToString();
					return (user.HashedPass == hash) ;
				}
            }
			return false; 
		}

		/// <summary>
		/// Inserts a user into the Users table in the HangmanDB.accdb file
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="hashedPass"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public int InsertUser(string userName, string hashedPass, string salt)
        {
			using (OdbcConnection odbcConnection = new OdbcConnection(ConnectionString))
			{
				OdbcCommand command = odbcConnection.CreateCommand();
				command.CommandText = "INSERT INTO Users (username, hash, salt) VALUES ('" + userName + "', '" + hashedPass + "', '" + salt + "')";
				try
				{
					odbcConnection.Open();
					return command.ExecuteNonQuery();
				}
				catch(Exception ex)
                {
					throw new Exception(ex.Message, ex);
                }
			}
        }

		/// <summary>
		/// Example function from the tutorial I was following
		/// returns a list of the rows in the .accdb file
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static List<string> ProcessFile()
		{
			string folderPath = "./assets/";
			//the file pattern is *output.accdb
			var file = Directory.GetFiles(@folderPath, "*HangmanDB.accdb").FirstOrDefault();
			string connectionstring = null;
			if (File.Exists(file))
			{
				connectionstring = @"Driver={Microsoft Access Driver 
						  (*.mdb, *.accdb)};DBQ=" + file;
				OdbcConnection odbcConnection = new OdbcConnection(connectionstring);
				try
				{
					odbcConnection.Open();
					List<string> tableNames = new List<string>();
					var schema = odbcConnection.GetSchema("Tables");

					foreach (System.Data.DataRow row in schema.Rows)
					{
						var tableName = row["TABLE_NAME"].ToString();
						//Exclude the system tables
						if (!tableName.StartsWith("MSys"))
						{
							tableNames.Add(tableName);
						}
					}

					foreach (var tableName in tableNames)
					{
						Console.WriteLine(tableName);
					}
					odbcConnection.Close();
					return tableNames;
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			return null;
		}
	}
}
