using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotwScripts.Lib
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate string Input(string ask);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate bool Option(string ask);

    /// <summary>
    /// 
    /// </summary>
    public delegate void Notify(object message);

    public static class CLI
    {
        public static string Input(string ask)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(ask);
            var user = Console.ReadLine();

            while (user == "" || user == null)
            {
                Console.Write($"Invalid responce -\n\n{ask}");
                user = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            return user;
        }

        public static bool Option(string ask)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(ask);
            ConsoleKeyInfo answer = Console.ReadKey();

            while (answer.Key != ConsoleKey.Y && answer.Key != ConsoleKey.N)
            {
                Console.Write("\b \b");
                answer = Console.ReadKey();
            }

            ConsoleKeyInfo consoleKey = Console.ReadKey();

            while (consoleKey.Key != ConsoleKey.Enter)
            {
                Console.Write("\b \b");
                consoleKey = Console.ReadKey();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return answer.Key == ConsoleKey.Y;
        }

        public static void WriteLine(object message)
        {
            if (message.ToString().Contains("||"))
            {
                string[] args = message.ToString().Split("||", 2);

                if (args.Length == 2)
                {
                    message = args[1];
                    Console.ForegroundColor = args[0] switch
                    {
                        "!warn" => ConsoleColor.DarkYellow,
                        "!error" => ConsoleColor.DarkRed,
                        "!notice" => ConsoleColor.Cyan,
                        "!good" => ConsoleColor.Green,
                        _ => ConsoleColor.Blue,
                    };
                }
            }
            
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
