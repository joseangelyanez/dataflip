using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    public class CodeException : Exception
    {
        public CodeException(string message)
            :
            base(message)
        { }
    }
}
