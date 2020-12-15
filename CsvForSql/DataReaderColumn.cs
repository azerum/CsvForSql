using System;

namespace CsvForSql
{
    public class DataReaderColumn
    {
        public string Name { get; }
        public Type DataType { get; }

        public Guid Guid { get; }

        public DataReaderColumn(string name, Type dataType)
        {
            Name = name;
            DataType = dataType;

            Guid = Guid.NewGuid();
        }
    }
}
