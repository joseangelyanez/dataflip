using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Represents the information to produce one file that connects to the a database
    /// and has methods for all the sprocs defined in the dataflip.json file.
    /// </summary>
    public class Context
    {
        public Context(ContextSection section)
        {
            Section = section;
        }

        /// <summary>
        /// Gets the configuration section that produces this context.
        /// </summary>
        public ContextSection Section { get; set; }
        /// <summary>
        /// Gets or sets the automatically generated code "namespace" element.
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// Gets or sets the automatically generated code "class" element.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a list with all the methods pending for code generation.
        /// </summary>
        public List<SprocMethod> Methods { get; set; } = new List<SprocMethod>();
        
    }
}
