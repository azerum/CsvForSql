using System;

namespace CsvForSql.CsvReading
{
    public static class HexDecoder
    {
        /// <summary>
        /// Декодирует строку в шестнадцатеричном коде в массив байт.
        /// </summary>
        /// <param name="hexString">
        /// Строка в шестнадцатеричном коде. Строка не должна содержать
        /// префикс 0x, только закодированные байты.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Строка содержит знак минус -
        /// или
        /// Строка состоит из нечетного количества символов.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Строка является null. 
        /// </exception>
        /// <exception cref="FormatException">
        /// Строка содержит символы, недопустимые для шестнадцатеричного кода.
        /// </exception>
        public static byte[] DecodeHexString(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Hexadecimal string must contain even number of characters.");
            }

            int resultLength = hexString.Length / 2;
            byte[] result = new byte[resultLength];

            char[] numeral = new char[2];

            for (int i = 0; i < resultLength; ++i)
            {
                hexString.CopyTo(i * 2, numeral, 0, 2);
                result[i] = Convert.ToByte(new string(numeral), 16);
            }

            return result;
        }
    }
}
