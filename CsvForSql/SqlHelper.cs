using CsvForSql.CsvReading;

using System.Data;
using System.Data.SqlClient;

namespace CsvForSql
{
    public static class SqlHelper
    {
        public static bool CheckIfTableExistsInDatabase(SqlConnection connection, string tableName)
        {
            string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @table_name";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@table_name", tableName);

            int tablesCount = (int)command.ExecuteScalar();

            return tablesCount > 0;
        }

        public static void ImportCsvToTable(string csvFilePath, SqlConnection connection, string tableName)
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

        public static DataTable GetSchemaOfTable(SqlConnection connection, string tableName)
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
