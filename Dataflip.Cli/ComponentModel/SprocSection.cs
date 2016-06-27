using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Represents a configuration section that defines a sproc.
    /// </summary>
    public class SprocSection
    {
        /// <summary>
        /// Constructs a SprocSection from the current context configuration.
        /// </summary>
        /// <param name="configuration">The current Context section.</param>
        public SprocSection(ContextSection configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The context configuration the sproc belongs to.
        /// </summary>
        public ContextSection Configuration { get; set; }
        /// <summary>
        /// The name of the sproc as it sits in SQL Server and set in the "name" section of the dataflip.json file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the automatically generated method and set in the "method" section of the dataflip.json file.
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// The return type for the sproc and set in the "return" section of the dataflip.json file.
        /// </summary>
        public string Return { get; set; }
        /// <summary>
        /// The comments to use on top of the sproc method and set in the "comments" section of the dataflip.json file.
        /// </summary>
        public string Comments { get; set; }    
        /// <summary>
        /// The test parameters used to retrieve the result properties and set in the "testParams" section of the dataflip.json file.
        /// </summary>
        public List<TestParamSection> TestParams { get; set; } = new List<TestParamSection>();
    }
}
