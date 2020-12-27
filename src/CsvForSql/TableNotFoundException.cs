using System;

namespace CsvForSql
{
    public class TableNotFoundException : Exception
    {
        public TableNotFoundException(string tableName)
            : base($"Table \"{tableName}\" not found in database.")
        {

        }
    }
}
