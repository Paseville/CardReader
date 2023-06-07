using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCardReader
{
    internal class APIRequestSchema
    {
        public string mac { get; set; }
        public string source { get; set; }
        public DateTime time { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string password { get; set; }

       public APIRequestSchema(string user, string password) { 
            this.user = user;
            this.password = password;
            this.time = DateTime.Now;
            this.status = "0";
            this.mac = "0";
            this.source = "4";
        }  
    }
}
