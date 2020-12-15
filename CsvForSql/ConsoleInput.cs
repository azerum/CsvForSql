using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvForSql
{
    public static class ConsoleInput
    {
        public static string AskString(string promtMessage)
        {
            Console.WriteLine($"{promtMessage} > ");
            return Console.ReadLine();
        }

        public static int ChooseFromList(string listPromt, params int[] choices)
        {
            Console.WriteLine(listPromt);

            int choice;

            while (true)
            {
                ClearPreviousLine();

                Console.WriteLine("> ");
                string input = Console.ReadLine();

                if (Int32.TryParse(input, out choice))
                {
                    if (choices.Contains(choice))
                    {
                        break;
                    }
                }
            }

            return choice;
        }

        private static void ClearPreviousLine()
        {
            int cursorTop = Console.CursorTop;
            int cursorLeft = Console.CursorLeft;

            Console.SetCursorPosition(0, Math.Max(0, cursorTop - 1));
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}
