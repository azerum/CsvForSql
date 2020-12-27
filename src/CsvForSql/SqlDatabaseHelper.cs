using CsvForSql.CsvReading;

using System.Data;
using System.Data.SqlClient;

namespace CsvForSql
{
    public class SqlDatabaseHelper
    {
        private SqlConnection connection;

        public SqlDatabaseHelper(SqlConnection openedConnection)
        {
            connection = openedConnection;
        }

        public bool CheckIfTableExists(string tableName)
        {
            string query = @"SELECT COUNT(*) 
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE table_name = @table_name";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@table_name", tableName);

            int tablesCount = (int)command.ExecuteScalar();

            return tablesCount > 0;
        }

        /// <exception cref="TableNotFoundException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="MalformedLineException"/>
        /// <exception cref="HeaderColumnNotFoundInTableException"/>
        /// <exception cref="FormatException"/>
        public void ImportCsvToTable(string csvFilePath, string tableName)
        {
            if (!CheckIfTableExists(tableName))
            {
                throw new TableNotFoundException(tableName);
            }

            using (SqlCsvReader reader = new SqlCsvReader(csvFilePath, GetSchemaOfTable(tableName)))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = tableName;

                foreach (CsvReaderColumn column in reader.Header)
                {
                    bulkCopy.ColumnMappings.Add(column.Name, column.Name);
                }

                bulkCopy.WriteToServer(reader);
            }
        }

        private DataTable GetSchemaOfTable(string tableName)
        {
            //Параметр tableName должен быть проверен методом CheckIfTableExists().
            //Если проверка пройдена, то в tableName нет SQL-инъекции.

            string query = $"SELECT * FROM {tableName};";

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
