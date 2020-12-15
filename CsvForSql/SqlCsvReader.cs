using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace CsvForSql
{
    public class SqlCsvReader : IDataReader
    {
        private TextFieldParser csvParser;
        private object[] currentRow;

        public ReadOnlyCollection<DataReaderColumn> Header { get; }

        public int FieldCount => Header.Count;

        public object this[string name] => currentRow[GetOrdinal(name)];
        public object this[int i] => currentRow[i];

        public int Depth => 1;
        public int RecordsAffected { get; private set; }
        public bool IsClosed { get; private set; }

        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="HeaderColumnNotFoundInTableException"/>
        public SqlCsvReader(string filePath, DataTable schemaTable, string csvDelimeter = ",")
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            csvParser = new TextFieldParser(filePath);
            csvParser.SetDelimiters(csvDelimeter);

            Header = ReadHeaderColumnsAndMatchThemInSchemaTable(csvParser, schemaTable).AsReadOnly();

            currentRow = new object[FieldCount];
            RecordsAffected = 0;
            IsClosed = false;
        }

        private List<DataReaderColumn> ReadHeaderColumnsAndMatchThemInSchemaTable(TextFieldParser csvParser, 
                                                                                  DataTable schemaTable)
        {
            List<DataReaderColumn> matchedColums = new List<DataReaderColumn>();
            string[] columnNames = csvParser.ReadFields();

            foreach (string columnName in columnNames)
            {
                DataColumn column = schemaTable.Columns[columnName];

                if (column == null)
                {
                    throw new HeaderColumnNotFoundInTableException(columnName);
                }    

                Type columnDataType = column.DataType;
                matchedColums.Add(new DataReaderColumn(columnName, columnDataType));
            }

            return matchedColums;
        }

        public bool Read()
        {
            if (csvParser.EndOfData)
            {
                return false;
            }

            string[] fields = csvParser.ReadFields();
            SetCurrentRowValues(fields);

            ++RecordsAffected;

            return true;
        }

        private void SetCurrentRowValues(string[] fields)
        {
            for (int columnOrdinal = 0; columnOrdinal < FieldCount; ++columnOrdinal)
            {
                currentRow[columnOrdinal] = CastFieldToColumnValue(fields[columnOrdinal], columnOrdinal);          
            }
        }

        private object CastFieldToColumnValue(string field, int columnOrdinal)
        {
            if (String.IsNullOrEmpty(field))
            {
                return null;
            }

            object columnValue;
            Type columnDataType = Header[columnOrdinal].DataType;

            if (columnDataType.Equals(typeof(byte[])))
            {
                columnValue = HexDecoder.DecodeHexString(field);
            }
            else
            {
                columnValue = Convert.ChangeType(field, columnDataType);
            }

            return columnValue;
        }

        public bool NextResult()
        {
            return false;
        }

        public void Close()
        {
            if (!IsClosed)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            IsClosed = true;

            csvParser.Close();
            csvParser = null;
        }

        public int GetOrdinal(string name)
        {
           foreach (DataReaderColumn column in Header)
           {
                if (column.Name == name)
                {
                    return Header.IndexOf(column);
                }
           }

            throw new ArgumentException($"Cannot find column with name {name}.");
        }

        public string GetName(int i)
        {
            return Header[i].Name;
        }

        public Type GetFieldType(int i)
        {
            return Header[i].DataType;
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        public Guid GetGuid(int i)
        {
            return Header[i].Guid;
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public object GetValue(int i)
        {
            return currentRow[i];
        }

        public int GetValues(object[] values)
        {
            Array.Copy(currentRow, values, FieldCount);
            return FieldCount;
        }

        public bool GetBoolean(int i)
        {
            return (bool)currentRow[i];
        }

        public float GetFloat(int i)
        {
            return (float)currentRow[i];
        }

        public double GetDouble(int i)
        {
            return (float)currentRow[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)currentRow[i];
        }

        public short GetInt16(int i)
        {
            return (short)currentRow[i];
        }

        public int GetInt32(int i)
        {
            return (int)currentRow[i];
        }

        public long GetInt64(int i)
        {
            return (long)currentRow[i];
        }

        public byte GetByte(int i)
        {
            return (byte)currentRow[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            //TODO: реализовать метод, если понадобится.
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)currentRow[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            //TODO: реализовать метод, если понадобится.
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)currentRow[i];
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)currentRow[i];
        }

        public bool IsDBNull(int i)
        {
            return (currentRow[i] == null);
        }

        public DataTable GetSchemaTable()
        {
            DataTable schemaTable = new DataTable();

            foreach (DataReaderColumn headerColumn in Header)
            {
                schemaTable.Columns.Add(headerColumn.Name, headerColumn.DataType);
            }

            return schemaTable;
        }
    }
}
