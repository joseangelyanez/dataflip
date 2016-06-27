using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Defines a resolved parameter for a sproc.
    /// </summary>
    public class SprocParameter
    {
        /// <summary>
        /// Gets or sets the parameter name as it appears in the sproc.
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// Gets or sets the CLR type that this parameter should be mapped to.
        /// </summary>
        public Type ClrType { get; set; }
    }
}
