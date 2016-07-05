using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    /// <summary>
    /// Represents a class that can receive a Context object and produce C# code.
    /// </summary>
    public class CSharpContextGenerator
    {
        /// <summary>
        /// The current context.
        /// </summary>
        public Context CurrentContext { get; set; }

        /// <summary>
        /// Creates a new instance of the CSharpContextGenerator
        /// </summary>
        /// <param name="currentContext">The current context.</param>
        public CSharpContextGenerator(Context currentContext)
        {
            if (currentContext == null)
                throw new ArgumentNullException("currentContext");

            CurrentContext = currentContext;
        }

        /// <summary>
        /// Generates the code for the current context.
        /// </summary>
        /// <returns>The code for the current context.</returns>
        public string GenerateCode()
        {
            CodeWriter code = new CodeWriter();

            code.WriteCode("using System;");
            code.WriteCode("using System.Linq;");
            code.WriteCode("using Dataflip;");
            code.WriteCode("using System.Collections.Generic;");
            code.WriteLine();

            code.WriteCode($"namespace {CurrentContext.Namespace}");
            code.WriteCode("{");

            /* Context Class */
            code.WriteCode($"public class {CurrentContext.Name} : DataflipContext");
            code.WriteCode("{");

            code.WriteCode($"public {CurrentContext.Name}(DataflipSettings settings)");
            code.WriteCode(": ");
            code.WriteCode("base(settings)");
            code.WriteCode("{}");
            code.WriteLine();
            code.WriteCode($"public {CurrentContext.Name}()");
            code.WriteCode(": ");
            code.WriteCode($"base(\"{CurrentContext.Name}\")");
            code.WriteCode("{}");
            code.WriteLine();

            /* Result Classes */
            code.WriteCode("#region Result Classes");
            foreach (var method in CurrentContext.Methods.Where(n => n.ReturnType == SprocMethod.ReturnTypes.IEnumerableOf))
            {
                code.WriteCode($"public class {method.MethodName}Result");
                code.WriteCode("{");   
                foreach (var property in method.ResultProperties)
                {
                    code.WriteCode($"public {property.ClrType.GetFriendlyName()} {property.PropertyName} {{ get; set; }} ");
                }
                code.WriteCode("}");
                code.WriteLine();
            }
            code.WriteCode("#endregion");
            code.WriteLine();
            code.WriteLine();

            /* Methods */
            foreach (var method in CurrentContext.Methods)
            {
                code.WriteCode($"#region {method.SprocName}");

                

                code.WriteCode($"public class {method.MethodName}Parameters");
                code.WriteCode("{");

                foreach (var param in method.Parameters)
                {
                    code.WriteCode($"public {param.ClrType.GetFriendlyName()} {param.ParameterName.Replace("@", "")} {{ get; set; }} ");
                }

                code.WriteCode("}");

                if (method.Comments != null)
                {
                    code.WriteCode("///<summary>");
                    code.WriteCode($"///{ method.Comments }");
                    code.WriteCode("///</summary>");
                }

                if (method.ReturnType == SprocMethod.ReturnTypes.IEnumerableOf)
                    code.WriteCode("public IEnumerable<{0}Result> {0}({1})", 
                        method.MethodName,
                        method.Parameters.Count() == 0 ? "" : string.Format("{0}Parameters parameters", method.MethodName)
                    );

                if (method.ReturnType == SprocMethod.ReturnTypes.IntWithRecordsAffected)
                    code.WriteCode("public int {0}({1})",
                        method.MethodName,
                        method.Parameters.Count() == 0 ? "" : string.Format("{0}Parameters parameters", method.MethodName)
                    );

                if (method.ReturnType == SprocMethod.ReturnTypes.ScalarValue)
                    code.WriteCode("public {2} {0}({1})",
                        method.MethodName,
                        method.Parameters.Count() == 0 ? "" : string.Format("{0}Parameters parameters", method.MethodName),
                        method.Return.Name
                    );

                code.WriteCode("{"); 
                if (method.ReturnType == SprocMethod.ReturnTypes.IEnumerableOf)    
                    code.WriteCode("return new SqlQuery(Settings).ExecuteObjectArray(");
                if (method.ReturnType == SprocMethod.ReturnTypes.IntWithRecordsAffected)
                    code.WriteCode("return new SqlQuery(Settings).ExecuteNonQuery(");
                if (method.ReturnType == SprocMethod.ReturnTypes.ScalarValue)
                    code.WriteCode($"return ({method.Return.Name}) new SqlQuery(Settings).ExecuteScalar(");

                code.BeingIndentation();
                code.WriteCode($"query : \"{method.SprocName}\",");
                code.WriteCode("parameters : _ =>");
                code.WriteCode("{");
                code.WriteCode("if (parameters == null) return;");
                code.WriteLine();

                foreach (var param in method.Parameters)
                {
                    code.WriteCode($"_.AddWithValue(\"{param.ParameterName}\", parameters.{param.ParameterName.Replace("@", "")});");
                }
                if (method.ReturnType == SprocMethod.ReturnTypes.IEnumerableOf)
                {
                    code.WriteCode("},");
                    code.WriteCode($"mapping: reader => new {method.MethodName}Result()");
                    code.WriteCode("{");

                    int counter = 0;
                    foreach (var param in method.ResultProperties)
                    {
                        counter++;
                        code.WriteCode("{0} = reader[\"{0}\"] as {1}{2}",
                            param.PropertyName,
                            param.ClrType.GetFriendlyName(),
                            counter < method.ResultProperties.Count ? "," : "");
                    }
                    code.WriteCode("}");
                }
                else
                {
                    code.WriteCode("}");
                }
                code.EndIndentation();
                code.WriteCode(");");
                code.WriteCode("}");
                code.WriteCode("#endregion");
                code.WriteLine();
            }

            code.WriteCode("}");
            code.WriteCode("}");

            return code.ToString();
        }
    }
}
