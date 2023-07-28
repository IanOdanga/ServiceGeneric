using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChequeTruncationService
{
    internal class getfiles
    {
        public List<string> GetAllFiles(string sDirt)
        {
            List<string> allFiles = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(sDirt))
                    allFiles.Add(file);
                foreach (string directory in Directory.GetDirectories(sDirt))
                    allFiles.AddRange((IEnumerable<string>)this.GetAllFiles(directory));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR====>" + ex.Message);
            }
            return allFiles;
        }
    }
}
