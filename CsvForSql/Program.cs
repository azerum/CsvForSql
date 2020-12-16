using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CsvForSql
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = InputDataAndOpenConnection();
            connection?.Close();
        }

        private static SqlConnection InputDataAndOpenConnection()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            SqlConnection connection = null;

            while (true)
            {
                connectionStringBuilder.DataSource = ConsoleInput.AskString("SQL Server instance name");
                Console.WriteLine();

                int authType = ConsoleInput.ChooseOption("1. Windows Authentication\n" +
                                                         "2. SQL Server Authentication\n",
                                                          1, 2);
                Console.WriteLine();

                if (authType == 1)
                {
                    connectionStringBuilder.IntegratedSecurity = true;
                }
                else
                {
                    connectionStringBuilder.UserID = ConsoleInput.AskString("User login");
                    connectionStringBuilder.Password = ConsoleInput.AskString("Password");

                    Console.WriteLine();
                }

                connectionStringBuilder.InitialCatalog = ConsoleInput.AskString("Database name");
                Console.WriteLine();

                connection = new SqlConnection(connectionStringBuilder.ToString());
                bool connectionOpened = TryToOpenConnection(connection);

                if (connectionOpened)
                {
                    break;
                }
            }

            return connection;
        }

        private static bool TryToOpenConnection(SqlConnection connection)
        {
            bool connectionOpened;

            while (true)
            {
                Console.WriteLine("Trying to connect to server...");

                try
                {
                    connection.Open();

                    connectionOpened = true;
                    break;
                }
                catch (SqlException)
                {
                    Console.WriteLine("Connection failed.");
                    Console.WriteLine();

                    string choice = ConsoleInput.ChooseOption("t - try again, i - input again, q - quit",
                                                              StringComparison.OrdinalIgnoreCase,
                                                              "t", "i", "q");
                    Console.WriteLine();

                    if (choice == "t")
                    {
                        continue;
                    }
                    else
                    {
                        connectionOpened = false;
                        break;
                    }
                }
            }

            return connectionOpened;
        }


        private static bool CheckIfTableExistsInDatabase(SqlConnection connection, string tableName)
        {
            string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @table_name";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@table_name", tableName);

            int tablesCount = (int)command.ExecuteScalar();

            return tablesCount > 0;
        }

        private static void ImportCsvToTable(string csvFilePath, SqlConnection connection, string tableName)
        {
            using (SqlCsvReader reader = new SqlCsvReader(csvFilePath, GetSchemaOfTable(connection, tableName)))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = tableName;

                foreach (DataReaderColumn column in reader.Header)
                {
                    bulkCopy.ColumnMappings.Add(column.Name, column.Name);
                }

                bulkCopy.WriteToServer(reader);
            }
        }

        private static DataTable GetSchemaOfTable(SqlConnection connection, string tableName)
        {
            //Параметр tableName должен быть проверен методом CheckIfTableExistsInDatabase().
            //Если проверка пройдена, то в tableName нет SQL-инъекции.

            string query = $"SELECT TOP(0) * FROM {tableName};";

            SqlCommand command = new SqlCommand(query, connection);
            DataTable schemaTable = new DataTable();

            using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                schemaTable.Load(reader);
            }

            return schemaTable;
        }
    }
}
