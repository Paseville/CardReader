using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCardReader
{/// <summary>
/// Exception which is thrown when a specified command is not registered in the APDUSender
/// </summary>
   class IllegalCommandException : Exception
    {
        public IllegalCommandException(string message)
        {
            Console.Write(message);
        }
    }
}
