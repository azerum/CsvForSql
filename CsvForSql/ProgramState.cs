namespace CsvForSql
{
    public enum ProgramState
    {
        InputtingConnectionString,
        TryingToConnectToServer,
        ConnectionFailed,
        ImportingCsvToDatabase,
        ImportFailed,
        Closing
    }
}
