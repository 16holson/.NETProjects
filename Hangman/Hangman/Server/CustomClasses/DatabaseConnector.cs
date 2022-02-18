using System.Data.Odbc;

namespace Hangman.Server.CustomClasses
{
    public class DatabaseConnector
    {
		public static void ProcessFile()
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
					if (odbcConnection is null)
                    {
						Console.WriteLine("odbcConnection failed to connect to .accdb file.");
                    }
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
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}
}
