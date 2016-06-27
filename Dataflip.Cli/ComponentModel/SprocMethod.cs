using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Defines a sproc object whose metadata has already been resolved.
    /// </summary>
    public class SprocMethod
    {
        public enum ReturnTypes
        {
            IEnumerableOf,
            IntWithRecordsAffected,
            ScalarValue
        }

        /// <summary>
        /// Defines the type of code generation that applies for this method.
        /// </summary>
        public ReturnTypes ReturnType { get; set; }
        /// <summary>
        /// Returns the automatically generated method name.
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// Returns the sproc as it will appear in the query parameter for the SqlQuery object.
        /// </summary>
        public string SprocName { get; set; }
        /// <summary>
        /// Returns the comments that will be added to the method.
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// Returns the CLR type that this method should return.
        /// </summary>
        public Type Return { get; set; }
        /// <summary>
        /// Returns the parameters that apply for this method.
        /// </summary>
        public IEnumerable<SprocParameter> Parameters { get; set; } = new List<SprocParameter>();
        /// <summary>
        /// Returns the list of properties that an object returned by this method should contain.
        /// </summary>
        public List<SprocResultProperty> ResultProperties { get; set; } = new List<SprocResultProperty>();
    }
}
