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
        public static bool HideOutput = false;
        public static async Task Call(string module, params string[] args)
        {
            Mtk.UpdateExternal(module);
            await Execute.App($"{Mtk.GetConfig("python")}\\python.exe", $"\"{Mtk.StaticPath}\\Scripts\\{module}\" {string.Join(' ', args)}", hidden: HideOutput, shellExecute: false);
        }
    }
}
