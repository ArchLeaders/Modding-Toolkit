using System;
using System.Collections.Generic;
using System.Linq;
using System.Operations;
using System.Text;
using System.Threading.Tasks;

namespace BotwScripts.Lib
{
    public class PythonInterop
    {
        public static void Call(string module, params string[] args)
        {
            Mtk.UpdateScript(module);
            Execute.App($"{Mtk.GetConfig("python")}\\python.exe", $"\"{Mtk.StaticPath}\\Scripts\\{module}.py\" {string.Join(' ', args)}");
        }
    }
}
