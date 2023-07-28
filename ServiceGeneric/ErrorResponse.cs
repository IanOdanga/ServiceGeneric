using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGeneric
{
    public class ErrorResponse
    {
        public int returnCode { get; set; }
        public string returnStatus { get; set; }
        public string errorSource { get; set; }
    }
}
