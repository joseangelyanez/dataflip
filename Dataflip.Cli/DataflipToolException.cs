using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli
{
    public class DataflipToolException : Exception
    {
        public DataflipToolException(string message)
            :
            base(message)
        {
        }
    }
}
