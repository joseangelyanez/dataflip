using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Represents the section for an entry in the "testParams" section within the dataflip.json file.
    /// </summary>
    public class TestParamSection
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
