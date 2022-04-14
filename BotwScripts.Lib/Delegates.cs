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
            Console.Write(ask);
            var user = Console.ReadLine();

            while (user == "" || user == null)
            {
                Console.Write($"Invalid responce -\n\n{ask}");
                user = Console.ReadLine();
            }

            return user;
        }
    }
}
