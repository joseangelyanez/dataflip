using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    /// <summary>
    /// Type > Extension class.
    /// </summary>
    public static class TypeExtentions
    {
        /// <summary>
        /// Returns the C# friendly nullable name for the Type.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>The type name with or without the "?".</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (type.IsValueType)
                return type.Name + "?";
            else
                return type.Name;
        }
    }
}
