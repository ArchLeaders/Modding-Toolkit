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
        public static async Task Call(string module, params string[] args, bool hidden = false)
        {
            Mtk.UpdateScript(module);
            await Execute.App($"{Mtk.GetConfig("python")}\\python.exe", $"\"{Mtk.StaticPath}\\Scripts\\{module}.py\" {string.Join(' ', args)}", hidden: hidden, shellExecute: false);
        }
    }
}
