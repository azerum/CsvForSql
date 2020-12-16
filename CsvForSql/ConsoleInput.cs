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
                ClearAllBetweenCursorAndLine(inputLineNumber);

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
            return ChooseOption(optionsPromt, StringComparison.Ordinal, options);
        }

        public static string ChooseOption(string optionsPromt, StringComparison optionsComparsion,
                                          params string[] options)
        {
            Console.WriteLine(optionsPromt);
            int inputLineNumber = Console.CursorTop;

            string choice;

            do
            {
                ClearAllBetweenCursorAndLine(inputLineNumber);

                Console.Write("> ");
                choice = Console.ReadLine();
            }
            while (!options.Any(option => String.Equals(option, choice, optionsComparsion)));

            return choice;
        }

        private static void ClearAllBetweenCursorAndLine(int lineNumber)
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
