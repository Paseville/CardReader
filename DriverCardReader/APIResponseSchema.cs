using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCardReader
{
    internal class APIResponseSchema
    {
        public string timestamp { get; set; }
        public int status { get; set; }
        public string nfcTag { get; set; }
    }
}
