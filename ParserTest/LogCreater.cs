using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTest
{
    internal class LogCreater
    {
        public void CreateLog(Exception ex, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, true)) 
            {
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine(ex.ToString());
                sw.WriteLine();
            }
        }

    }
}