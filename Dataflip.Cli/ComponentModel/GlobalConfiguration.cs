using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    public class GlobalConfiguration
    {
        /// <summary>
        /// Returns a GlobalConfiguration object from a valid json string.
        /// </summary>
        /// <param name="json">A valid json configuration string.</param>
        /// <returns>A GlobalConfiguration object.</returns>
        public static GlobalConfiguration FromJson(string json)
        {
            GlobalConfiguration configuration = new GlobalConfiguration();
            JObject jsonObject = JObject.Parse(json);

            if (jsonObject["contexts"] == null)
                throw new DataflipToolException("The 'contexts' array element in dataflip.json is required.");

            foreach (JObject contextObject in jsonObject["contexts"].AsJEnumerable())
            {
                ContextSection contextConfiguration = ContextSection.FromJson(contextObject);
                configuration.Contexts.Add(contextConfiguration);
            }

            return configuration;
        }

        /// <summary>
        /// Gets or sets a list with all the configured contexts in the file.
        /// </summary>
        public List<ContextSection> Contexts { get; set; } = new List<ContextSection>();
    }
}
