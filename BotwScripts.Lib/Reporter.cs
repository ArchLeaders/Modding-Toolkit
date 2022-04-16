using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotwScripts.Lib
{
    public class Reporter
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public int ID { get; set; }

        public void PrintToConsole()
        {

        }

        public Reporter(string message, string title = "Reported Exception", string stack = "", int id = 0)
        {
            Title = title;
            Message = message;
            Stack = stack;
            ID = id;
        }
    }
}
