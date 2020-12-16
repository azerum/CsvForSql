namespace CsvForSql
{
    public enum ProgramState
    {
        InputtingConnectionString,
        TryingToOpenConnection,
        ConnectionFailed,
        ConnectionOpened,
        Closing
    }
}
