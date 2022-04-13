using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acheron.Web;

namespace BotwScripts.Lib
{
    public class Python
    {
        public Python(string module, string args = "", string func = "[RUN.PYTHON]")
        {
            Mtk.CheckScriptVersion(module);
        }
    }
}
