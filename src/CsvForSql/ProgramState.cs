namespace CsvForSql
{
    public enum ProgramState
    {
        InputtingConnectionString,
        TryingToConnectToServer,
        ConnectionCancelled,
        ConnectionFailed,
        ImportingCsvToDatabase,
        ImportFailed,
        Closing
    }
}
