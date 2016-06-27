using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dataflip
{
    public class DataflipSettingsException : Exception
    {
        public DataflipSettingsException(string message)
            :
            base(message)
        { }
    }
}
