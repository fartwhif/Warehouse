using System;
using System.Collections.Generic;
using System.Text;

namespace Warehouse
{
    public partial class PluginCore
    {
        void writeErr(string s)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(ErrorLogFile, true);
            sw.WriteLine(s);
            sw.WriteLine("");
            sw.Close();
        }
    }
}
