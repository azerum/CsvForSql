using System;

namespace CsvForSql.CsvReading
{
    public class CsvReaderColumn
    {
        public string Name { get; }
        public Type DataType { get; }

        public Guid Guid { get; }

        public CsvReaderColumn(string name, Type dataType)
        {
            Name = name;
            DataType = dataType;

            Guid = Guid.NewGuid();
        }
    }
}
