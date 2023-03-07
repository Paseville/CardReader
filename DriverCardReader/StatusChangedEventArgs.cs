using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCardReader
{

    /// <summary>
    /// Simple event args to store the string the status text needs to be changed to
    /// </summary>
   public  class StatusChangedEventArgs : EventArgs
    {
        public string statusText;

        public StatusChangedEventArgs(string statusText)
        {
            this.statusText = statusText;
        }
    }
}
