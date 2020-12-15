using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CsvForSql
{
    enum SqlAutheticationType
    {
        WindowsAutentication,
        SqlServerAutentication
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Import CSV file to SQL Server database");
            Console.WriteLine();

            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            SqlConnection connection = null;

            bool inputtingConnection = true;

            while (inputtingConnection)
            {
                Console.Write("SQL Server instance name > ");
                connectionStringBuilder.DataSource = Console.ReadLine();

                Console.WriteLine();

                Console.WriteLine("Choose type of authentication: ");
                Console.WriteLine("1. Windows Authentication");
                Console.WriteLine("2. SQL Server Authentication");
                Console.Write("> ");
             

                Console.WriteLine();

                if (input == "1")
                {
                    connectionStringBuilder.IntegratedSecurity = true;
                }
                else
                {
                    Console.Write("Login > ");
                    connectionStringBuilder.UserID = Console.ReadLine();

                    Console.Write("Password > ");
                    connectionStringBuilder.Password = Console.ReadLine();

                    Console.WriteLine();
                }

                Console.Write("Database name > ");
                connectionStringBuilder.InitialCatalog = Console.ReadLine();

                bool tryingToConnect = true;

                while (tryingToConnect)
                {
                    Console.WriteLine("Trying to connect...");

                    try
                    {
                        connection = new SqlConnection(connectionStringBuilder.ToString());
                        connection.Open();

                        Console.WriteLine("Success!");
                        Console.WriteLine();

                        tryingToConnect = false;
                        inputtingConnection = false;
                    }
                    catch (SqlException)
                    {
                        Console.WriteLine("Connection failed.");

                        Console.WriteLine();

                        Console.WriteLine("t - try again, i - input information again, q - quit (default - q)");
                        Console.Write("> ");

                        input = Console.ReadLine();

                        switch (input)
                        {
                            case "t":
                            case "T":
                                tryingToConnect = true;
                                inputtingConnection = true;
                                break;

                            case "i":
                            case "I":
                                tryingToConnect = false;
                                inputtingConnection = true;
                                break;

                            case "q":
                            case "Q":
                            default:
                                return;
                        }

                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine();

            string tableName = InputTableName(connection);
            string csvFilePath = InputCsvFilePath();

            try
            {
                ImportCsvToTable(csvFilePath, connection, tableName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (HeaderColumnNotFoundInTableException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MalformedLineException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

        private static SqlAutheticationType InputAutheticationType()
        {
            string input;

            do
            {
                input = Console.ReadLine();

                int? choise = null;
                Int32.TryParse(input, out choise);
            }
            while (cho != "1" && input != "2");
        }

        private static string InputTableName(SqlConnection connection)
        {
            string tableName = default;
            bool inputtingTableName = true;

            while (inputtingTableName)
            {
                Console.Write("Table name > ");
                tableName = Console.ReadLine();

                if (!CheckIfTableExistsInDatabase(connection, tableName))
                {
                    Console.WriteLine("Table not found.");
                    Console.WriteLine();
                }
                else
                {
                    inputtingTableName = false;
                }
            }

            return tableName;
        }

        private static bool CheckIfTableExistsInDatabase(SqlConnection connection, string tableName)
        {
            string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @table_name";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@table_name", tableName);

            int tablesCount = (int)command.ExecuteScalar();

            return tablesCount > 0;
        }

        private static string InputCsvFilePath()
        {
            string csvFilePath = default;
            bool inputtingFilePath = true;

            while (inputtingFilePath)
            {
                Console.Write("Csv file name > ");
                csvFilePath = Console.ReadLine().Trim('\'', '"');

                if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine("File not found or cannot be accesed.");
                }
                else
                {
                    inputtingFilePath = false;
                }

                Console.WriteLine();
            }

            return csvFilePath;
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
