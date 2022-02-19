using System.Data.Odbc;

namespace Hangman.Server.CustomClasses
{
    public class DatabaseConnector
    {
		public string ConnectionString { get; set; }
		public string FolderPath { get; set; }
		public string FileName { get; set; }

		public DatabaseConnector(string filepath, string filename)
        {
			FolderPath = filepath;	
			this.FileName = filename;
			var file = Directory.GetFiles(filepath, filename).FirstOrDefault();
			if(File.Exists(file))
            {
				ConnectionString = @"Driver={Microsoft Access Driver 
						  (*.mdb, *.accdb)};DBQ=" + file ;
			}
		}

		public void Connect()
        {
            try 
			{ 
				OdbcConnection odbcConnection = new OdbcConnection(ConnectionString);
				odbcConnection.Open();
			}
			catch (Exception ex)
            {
				throw new Exception(ex.Message);
            }

        }

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

					// foreach (var tableName in tableNames)
					// {
					// 	Console.WriteLine(tableName);
					// }
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
