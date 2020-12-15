using System;

namespace CsvForSql
{
    public static class HexDecoder
    {
        /// <summary>
        /// Decodes string with hexadecimal digits to array of bytes.
        /// </summary>
        /// <param name="hexString">
        /// String with hexadecimal digits. It must not include 0x prefix,
        /// only hexadimal digits.
        /// </param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
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
