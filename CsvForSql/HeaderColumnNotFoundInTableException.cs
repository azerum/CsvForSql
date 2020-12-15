using System;

namespace CsvForSql
{
    public class HeaderColumnNotFoundInTableException : Exception
    { 
        public HeaderColumnNotFoundInTableException(string columnName) :
            base($"Column \"{columnName}\" from csv header not found in table.")
        {

        }
    }
}
