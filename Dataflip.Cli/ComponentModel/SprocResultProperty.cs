using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Represents a property within a result returned by a sproc method.
    /// </summary>
    public class SprocResultProperty
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Gets or sets the CLR type.
        /// </summary>
        public Type ClrType { get; set; }
    }
}
