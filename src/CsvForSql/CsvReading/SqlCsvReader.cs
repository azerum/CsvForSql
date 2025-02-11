﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace CsvForSql.CsvReading
{
    /// <summary>
    /// Реализация интерфейса <see cref="IDataReader"/> для чтения данных SQL Server из CSV файла.
    /// </summary>
    /// <remarks>
    /// Для записи файла в таблицу используется класс SqlBulkCopy. У него есть
    /// метод, копирующий в таблицу данные из IDataReader.
    /// 
    /// SqlDataReader реализует IDataReader и передает считанные из файла 
    /// значения к  SqlBulkCopy.
    ///
    /// Для возврата значений с правильными типами, SqlDataReader сопоставляет
    /// заголовок CSV файла со столбцами таблицы в базе.
    /// Класс построчно читает файл и приводит считанные строки к типам столбцов
    /// таблицы.
    /// 
    /// Код записи файла находится в классе SqlHelper.
    /// </remarks>
    public class SqlCsvReader : IDataReader
    {
        private TextFieldParser csvParser;
        private object[] currentRow;

        public ReadOnlyCollection<CsvReaderColumn> Header { get; }

        public int FieldCount => Header.Count;

        public object this[string name] => currentRow[GetOrdinal(name)];
        public object this[int i] => currentRow[i];

        public int Depth => 1;
        public int RecordsAffected { get; private set; }
        public bool IsClosed { get; private set; }

        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="MalformedLineException"/>
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

        /// <exception cref="MalformedLineException"/>
        /// <exception cref="HeaderColumnNotFoundInTableException"/>
        private List<CsvReaderColumn> ReadHeaderColumnsAndMatchThemInSchemaTable(TextFieldParser csvParser, 
                                                                                 DataTable schemaTable)
        {
            List<CsvReaderColumn> matchedColums = new List<CsvReaderColumn>();
            string[] columnNames = csvParser.ReadFields();

            foreach (string columnName in columnNames)
            {
                DataColumn dataColumn = schemaTable.Columns[columnName];

                if (dataColumn == null)
                {
                    throw new HeaderColumnNotFoundInTableException(columnName);
                }    

                Type columnDataType = dataColumn.DataType;
                matchedColums.Add(new CsvReaderColumn(columnName, columnDataType));
            }

            return matchedColums;
        }

        /// <exception cref="MalformedLineException"/>
        /// <exception cref="FormatException"/>
        public bool Read()
        {
            if (csvParser.EndOfData)
            {
                return false;
            }

            string[] values = csvParser.ReadFields();

            SetCurrentRowValues(values);

            ++RecordsAffected;

            return true;
        }

        /// <exception cref="FormatException"/>
        private void SetCurrentRowValues(string[] values)
        {
            for (int columnOrdinal = 0; columnOrdinal < FieldCount; ++columnOrdinal)
            {
                currentRow[columnOrdinal] = CastStringToColumnValue(values[columnOrdinal], columnOrdinal);          
            }
        }

        /// <exception cref="FormatException"/>
        private object CastStringToColumnValue(string s, int columnOrdinal)
        {
            if (String.IsNullOrEmpty(s))
            {
                return null;
            }

            object columnValue = null;
            Type columnDataType = Header[columnOrdinal].DataType;

            if (columnDataType.Equals(typeof(byte[])))
            {
                try
                {
                    columnValue = HexDecoder.DecodeHexString(s);
                }
                catch (Exception ex) when (IsHexDecoderException(ex))
                {
                    ThrowBinaryColumnFormatException(Header[columnOrdinal].Name, s);
                }
            }
            else
            {
                columnValue = Convert.ChangeType(s, columnDataType);
            }

            return columnValue;
        }

        private bool IsHexDecoderException(Exception ex)
        {
            return ex is ArgumentException ||
                   ex is FormatException;
        }

        private void ThrowBinaryColumnFormatException(string columnName, string value)
        {
            string message = $"Binary column {columnName} " +
                             $"contains invalid hexadecimal value - \"{value}\".";

            throw new FormatException(message);
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

        /// <exception cref="ArgumentException"/>
        public int GetOrdinal(string name)
        {
           foreach (CsvReaderColumn column in Header)
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

        /// <exception cref="NotSupportedException"/>
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

            foreach (CsvReaderColumn headerColumn in Header)
            {
                schemaTable.Columns.Add(headerColumn.Name, headerColumn.DataType);
            }

            return schemaTable;
        }
    }
}
