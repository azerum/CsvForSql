using System;
using System.Linq;

namespace CsvForSql
{
    public static class ConsoleInput
    {
        public static string AskString(string promtMessage)
        {
            Console.Write($"{promtMessage} > ");
            return Console.ReadLine();
        }

        public static int ChooseOption(string optionsPromt, params int[] options)
        {
            Console.WriteLine(optionsPromt);
            int inputLineNumber = Console.CursorTop;

            int choice;

            while (true)
            {
                ClearAllBetweenCursorAndLineWithNumber(inputLineNumber);

                Console.Write("> ");
                string input = Console.ReadLine();

                if (Int32.TryParse(input, out choice))
                {
                    if (options.Contains(choice))
                    {
                        break;
                    }
                }
            }

            return choice;
        }

        public static string ChooseOption(string optionsPromt, params string[] options)
        {
            return ChooseOption(optionsPromt, ignoreCase: true, options);
        }

        public static string ChooseOption(string optionsPromt, bool ignoreCase,
                                          params string[] options)
        {
            Console.WriteLine(optionsPromt);
            int inputLineNumber = Console.CursorTop;

            string choice;

            StringComparison optionsComparison = (ignoreCase) ?
                                                 StringComparison.OrdinalIgnoreCase :
                                                 StringComparison.Ordinal;

            do
            {
                ClearAllBetweenCursorAndLineWithNumber(inputLineNumber);

                Console.Write("> ");
                choice = Console.ReadLine();
            }
            while (!options.Any(option => String.Equals(option, choice, optionsComparison)));

            return choice;
        }

        private static void ClearAllBetweenCursorAndLineWithNumber(int lineNumber)
        {
            int lastLineNumber = Console.CursorTop;
            string cleanLine = new string(' ', Console.BufferWidth);

            for (int line = lineNumber; line <= lastLineNumber; ++line)
            {
                Console.SetCursorPosition(0, line);
                Console.Write(cleanLine);
            }

            Console.SetCursorPosition(0, lineNumber);
        }
    }
}
