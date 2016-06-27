using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip
{
    public class DataflipConfigurationEventArgs : EventArgs
    {
        public DataflipSettings Settings { get; set; }
        public string ContextName { get; set; }
    }
}
