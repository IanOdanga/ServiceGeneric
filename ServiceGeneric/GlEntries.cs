using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGeneric
{
    public class GlEntries
    {
        public string glCode { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string transactionID { get; set; }
        public string amount { get; set; }
        public int entryID { get; set; }
        public DateTime postingDate { get; set; }
        public DateTime documentDate { get; set; }
        public string accountNo { get; set; }
        public string currencyCode { get; set; }

    }
}
