using CsvForSql.CsvReading;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading;

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

            //Язык интерфейса программы - английский.
            //Устанавливаем нейтральную локаль для того, чтобы сообщения исключений
            //тоже выводились на английском.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private void Run()
        {
            bool running = true;

            WelcomeMessage();

            while (running)
            {
                switch (state)
                {
                    case ProgramState.InputtingConnectionString:
                        InputConnectionStringAndCreateConnection();
                        break;
                       
                    case ProgramState.TryingToConnectToServer:
                        TryToConnectToServer();
                        break;

                    case ProgramState.ConnectionFailed:
                        OnConnectionFailed();
                        break;

                    case ProgramState.ImportingCsvToDatabase:
                        ImportCsvToDatabase();
                        break;

                    case ProgramState.ImportFailed:
                        OnImportFailed();
                        break;

                    case ProgramState.Closing:
                        connection?.Close();
                        running = false;
                        break;
                }
            }
        }

        private void WelcomeMessage()
        {
            Console.WriteLine("Import csv file to SQL Server.");
            Console.WriteLine();
        }

        private void InputConnectionStringAndCreateConnection()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();

            connectionStringBuilder.DataSource = ConsoleInput.AskString("SQL Server instance name");
            Console.WriteLine();

            int authType = ConsoleInput.ChooseOption("Choose type of authentication\n\n" +
                                                     "1. Windows Authentication\n" +
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
            state = ProgramState.TryingToConnectToServer;
        }

        private void TryToConnectToServer()
        {
            Console.WriteLine("Trying to connect to server...");

            try
            {
                connection.Open();

                Console.WriteLine("Success!");
                Console.WriteLine();

                state = ProgramState.ImportingCsvToDatabase;
            }
            catch (SqlException)
            {
                Console.WriteLine("Connection failed.");
                state = ProgramState.ConnectionFailed;
            }
        }

        private void OnConnectionFailed()
        {
            Console.WriteLine();

            string choice = ConsoleInput.ChooseOption("t - try again, i - input again, q - quit",
                                                      "t", "i", "q");
            Console.WriteLine();

            switch (choice)
            {
                case "t":
                    state = ProgramState.TryingToConnectToServer;
                    break;

                case "i":
                    state = ProgramState.InputtingConnectionString;
                    break;

                case "q":
                    state = ProgramState.Closing;
                    break;
            }
        }

        private void ImportCsvToDatabase()
        { 
            string tableName = InputTableName();
            string csvFilePath = InputCsvFilePath();

            Console.WriteLine();

            try
            {
                SqlHelper.ImportCsvToTable(csvFilePath, connection, tableName);

                Console.WriteLine("Csv file successfully imported!");
                state = ProgramState.Closing;
            }
            catch (Exception ex) when (IsCsvException(ex))
            {
                Console.WriteLine($"Csv file error: {ex.Message}");
                state = ProgramState.ImportFailed;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL error: {ex.Message}");
                state = ProgramState.ImportFailed;
            }
        }

        private string InputTableName()
        {
            string tableName;

            while (true)
            {
                tableName = ConsoleInput.AskString("Table name");

                if (!SqlHelper.CheckIfTableExistsInDatabase(connection, tableName))
                {
                    Console.WriteLine($"Table \"{tableName}\" not found.");
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
                //При перетаскивании файла в консоль путь к нему может взяться в кавычки.
                //Их нужно убрать.
                csvFilePath = ConsoleInput.AskString("Csv file path").Trim('\'', '"');

                if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine("File not found.");
                    Console.WriteLine();
                }
                else
                {
                    break;
                }
            }

            return csvFilePath;
        }

        private bool IsCsvException(Exception ex)
        {
            return ex is HeaderColumnNotFoundInTableException ||
                   ex is MalformedLineException ||
                   ex is FormatException;
        }

        private void OnImportFailed()
        {
            Console.WriteLine();

            string choice = ConsoleInput.ChooseOption("i - input table name and file path again, q - quit",
                                                      "i", "q");

            switch (choice)
            {
                case "i":
                    state = ProgramState.ImportingCsvToDatabase;
                    break;

                case "q":
                    state = ProgramState.Closing;
                    break;
            }
        }
    }
}
