using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGeneric
{
    internal class UTILITIES
    {
        private static Collection<string> logs = new Collection<string>();
        public static string logpath = "C:\\LOGS\\Mambu Service Logs\\MambuService_logs";

        public static string LogFileName
        {
            get
            {
                if (!Directory.Exists(UTILITIES.logpath))
                    Directory.CreateDirectory(UTILITIES.logpath);
                return UTILITIES.logpath + DateTime.Now.ToString("yyyy-MMM-dd") + ".txt";
            }
        }

        private UTILITIES()
        {
        }

        public static void WriteLog(string clientRequest)
        {
            try
            {
                File.AppendAllText(UTILITIES.LogFileName, clientRequest + "\n");
            }
            catch
            {

            }
        }
    }
}
