using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Defines the part of the dataflip.json file that declares a context.
    /// </summary>
    public class ContextSection
    {
        /// <summary>
        /// Creates a context section from a valid JObject containing a context section.
        /// </summary>
        /// <param name="jsonObject">A valid context JObject object.</param>
        /// <returns>Returns an object containing the data set in the dataflip.json file for a context.</returns>
        public static ContextSection FromJson(JObject jsonObject)
        {
            ContextSection result = new ContextSection();
            try
            {
                string connectionString = jsonObject["connectionString"]?.Value<string>();
                string nspace = jsonObject["namespace"]?.Value<string>();
                string name = jsonObject["name"]?.Value<string>();
                string output = jsonObject["output"]?.Value<string>();
                string typeScriptTypings = jsonObject["typeScript"]?["output"]?.Value<string>();
                bool? useCamelCasing = jsonObject["typeScript"]?["useCamelCasing"]?.Value<bool>();
                string angularHtmlBindings = jsonObject["angular"]?["htmlBindings"]?.Value<string>();

                if (connectionString == null)
                    throw new DataflipToolException("The '/contexts[]/connectionString' element in dataflip.json is required.");
                if (nspace == null)
                    throw new DataflipToolException("The '/contexts[]/namespace' element in dataflip.json is required.");
                if (name == null)
                    throw new DataflipToolException("The '/contexts[]/name' element in dataflip.json is required.");
                if (output == null)
                    throw new DataflipToolException("The '/contexts[]/output' element in dataflip.json is required.");
                if (useCamelCasing == null)
                    useCamelCasing = true;
                
                result.ConnectionString = connectionString;
                result.Namespace = nspace;
                result.Name = name;
                result.Output = output;
                result.TypeScriptTypings = typeScriptTypings;
                result.TypeScriptUseCamelCasing = useCamelCasing.Value;
                result.AngularHtmlBindings = angularHtmlBindings;
            }
            catch (Exception ex)
            {
                throw new DataflipToolException("There was a problem parsing the configuration for at least one context, error details: " + ex.Message);
            }


            if (jsonObject["sprocs"] == null)
                throw new DataflipToolException("The context does not contain a 'sprocs' element. The 'sprocs' element is required.");

            foreach (var sproc in jsonObject["sprocs"].AsJEnumerable())
            {
                SprocSection sprocSection = new SprocSection(result) {
                    Name = sproc["name"].Value<string>(),
                    Return = sproc["return"]?.Value<string>(),
                    Method = sproc["method"]?.Value<string>(),
                    Comments = sproc["comments"]?.Value<string>()
                };

                if (sprocSection.Name == null)
                    throw new DataflipToolException("'contexts[]/sprocs[n]/name' section in dataflip.json is required.");

                if (sprocSection.Method == null)
                    sprocSection.Method = sprocSection.Name;

                result.Sprocs.Add(sprocSection);
                
                if (sproc["testParams"] != null)
                { 
                    foreach (var testParam in sproc["testParams"].AsJEnumerable())
                    {
                        var property = ((Newtonsoft.Json.Linq.JProperty)testParam.First);
                        if (property == null || property.Name == null || property.Value == null)
                            throw new DataflipToolException("There was a problem parsing the entry in 'testParams'.");

                        string paramName = property.Name;
                        string paramValue = property.Value.ToString();

                        sprocSection.TestParams.Add(new TestParamSection() {
                                Name = paramName,
                                Value = paramValue
                            }
                        );
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Returns the "connectionString" context section.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Returns the "namespace" context section.
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// Returns the "name" context section.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns the "output" context section.
        /// </summary>
        public string Output { get; set; }
        /// <summary>
        /// Returns the "sprocs" context section.
        /// </summary>
        public List<SprocSection> Sprocs { get; set; } = new List<SprocSection>();
        /// <summary>
        /// Gets or sets the TypeScript typings fot the current context.
        /// </summary>
        public string TypeScriptTypings { get; set; }
        /// <summary>
        /// Specifies whether or not camel casing should be used for TypeScript.
        /// </summary>
        public bool TypeScriptUseCamelCasing{ get; set; }
        /// <summary>
        /// Specifies the path for generating the HTML bindings file.
        /// </summary>
        public string AngularHtmlBindings { get; set; }
    }
}
