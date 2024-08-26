using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_run_assist_tool
{
    internal class initialization
    {
        public static void initialize()
        {
            Directory.CreateDirectory(main.directory() + "\\erat");
            Directory.CreateDirectory(main.directory() + "\\erat\\data");
            Directory.CreateDirectory(main.directory() + "\\erat\\database");
        }
    }
}
