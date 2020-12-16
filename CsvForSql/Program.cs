using System;
using System.Data.SqlClient;
using System.IO;

namespace CsvForSql
{
    public class Program
    {
        private ProgramState state;
        private SqlConnection connection;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        public Program()
        {
            state = ProgramState.InputtingConnectionString;
            connection = null;
        }

        private void Run()
        {
            bool running = true;

            while (running)
            {
                switch (state)
                {
                    case ProgramState.InputtingConnectionString:
                        InputConnectionStringAndCreateConnection();
                        break;
                       
                    case ProgramState.TryingToOpenConnection:
                        TryToOpenConnection();
                        break;

                    case ProgramState.ConnectionFailed:
                        OnConnectionFailed();
                        break;

                    case ProgramState.ConnectionOpened:
                        OnConnectionOpened();
                        break;

                    case ProgramState.Closing:
                        connection?.Close();
                        running = false;
                        break;
                }
            }
        }

        private void InputConnectionStringAndCreateConnection()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();

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
            state = ProgramState.TryingToOpenConnection;
        }

        private void TryToOpenConnection()
        {
            Console.WriteLine("Trying to connect to server...");

            try
            {
                connection.Open();
                state = ProgramState.ConnectionOpened;
            }
            catch (SqlException)
            {
                state = ProgramState.ConnectionFailed;
            }
        }

        private void OnConnectionFailed()
        {
            Console.WriteLine("Connection failed.");
            Console.WriteLine();

            string choice = ConsoleInput.ChooseOption("t - try again, i - input again, q - quit",
                                                      optionsComparison: StringComparison.OrdinalIgnoreCase,
                                                      "t", "i", "q");
            Console.WriteLine();

            switch (choice)
            {
                case "t":
                    state = ProgramState.TryingToOpenConnection;
                    break;

                case "i":
                    state = ProgramState.InputtingConnectionString;
                    break;

                case "q":
                    state = ProgramState.Closing;
                    break;
            }
        }

        private void OnConnectionOpened()
        {
            string tableName = InputTableName();
            string csvFilePath = InputCsvFilePath();

            try
            {
                SqlHelper.ImportCsvToTable(csvFilePath, connection, tableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            state = ProgramState.Closing;
        }

        private string InputTableName()
        {
            string tableName;

            while (true)
            {
                tableName = ConsoleInput.AskString("Table name");

                if (!SqlHelper.CheckIfTableExistsInDatabase(connection, tableName))
                {
                    Console.WriteLine($"Cannot find table \"{tableName}\".");
                    Console.WriteLine();
                }
                else
                {
                    break;
                }
            }

            return tableName;
        }

        private static string InputCsvFilePath()
        {
            string csvFilePath;

            while (true)
            {
                //При перетягивании файла в консоль путь к файлу может взяться в кавычки.
                //Убираем их.
                csvFilePath = ConsoleInput.AskString("Csv file path").Trim('\'', '"');

                if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine("Cannot find file.");
                    Console.WriteLine();
                }
                else
                {
                    break;
                }
            }

            return csvFilePath;
        }
    }
}
