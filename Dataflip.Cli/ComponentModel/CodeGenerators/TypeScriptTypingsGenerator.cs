using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    public class TypeScriptTypingsGenerator
    {
        public Context CurrentContext { get; set; }

        public TypeScriptTypingsGenerator(Context currentContext)
        {
            CurrentContext = currentContext;
        }

        public string GenerateCode()
        {
            CodeWriter code = new CodeWriter();

            code.WriteCode($"export namespace {CurrentContext.Name}");
            code.WriteCode("{");
            foreach (var x in CurrentContext.Methods)
            {
                if (x.ResultProperties != null)
                { 
                    code.WriteCode($"export class {x.MethodName}_Result");
                    code.WriteCode("{");

                    if (x.ResultProperties.Count() == 0)
                        code.WriteCode("/* Method returns a basic type. */");
                
                    foreach (var property in x.ResultProperties)
                    {
                        code.WriteCode($"{property.PropertyName.ToTypeScriptCasing(CurrentContext.Section.TypeScriptUseCamelCasing)} : {property.ClrType.ToTypeScript()} ;");
                    }
                    code.WriteCode("}");
                }
                if (x.Parameters != null)
                {
                    code.WriteCode($"export class {x.MethodName}_Parameters");
                    code.WriteCode("{");

                    if (x.Parameters.Count() == 0)
                        code.WriteCode("/* No parameters. */");

                    foreach (var parameter in x.Parameters)
                    {
                        code.WriteCode($"{parameter.ParameterName.ToTypeScriptCasing(CurrentContext.Section.TypeScriptUseCamelCasing)} : {parameter.ClrType.ToTypeScript()} ;");
                    }
                    code.WriteCode("}");
                }
                code.WriteLine();
            }
            code.WriteCode("}");

            return code.ToString();
        }
    }
}
