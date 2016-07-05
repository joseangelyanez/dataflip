using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    public class HtmlBindingsGenerator
    {
        public Context CurrentContext { get; set; }

        public HtmlBindingsGenerator(Context currentContext)
        {
            CurrentContext = currentContext;
        }

        public string GenerateCode()
        {
            CodeWriter code = new CodeWriter();

            code.WriteCode($"<!-- AngularJS2 Bindings -->");
            code.WriteLine();

            foreach (var x in CurrentContext.Methods)
            {
                if (x.ResultProperties != null)
                {
                    code.WriteCode($"<!-- Bindings for {x.MethodName}_Result -->");
                    
                    if (x.ResultProperties.Count() == 0)
                    {
                        code.WriteCode("<!-- Method returns a basic type. -->");
                    }
                    else
                    {
                        code.WriteLine();
                        code.WriteCode($"<!-- List binding for {x.MethodName} -->");
                        code.WriteCode("<table *ngFor=\"let item of ?? \">");
                        code.BeingIndentation();
                        code.WriteCode("<tr>");
                        code.BeingIndentation();
                        foreach (var property in x.ResultProperties)
                        {
                            string name = property.PropertyName.ToTypeScriptCasing(CurrentContext.Section.TypeScriptUseCamelCasing);
                            code.WriteCode($"<td>{{{{item.{name}}}}}</td>");
                        }
                        code.EndIndentation();
                        code.WriteCode("</tr>");
                        code.EndIndentation();
                        code.WriteCode("</table>");
                    }
                }

                if (x.Parameters != null)
                {
                    code.WriteCode($"<!-- Bindings for {x.MethodName}_Parameters -->");

                    if (x.Parameters.Count() == 0)
                    {
                        code.WriteCode("<!-- Method expects no parameters. -->");
                    }
                    else
                    {
                        code.WriteCode("<!-- n-Way Bindings -->");
                        foreach (var parameter in x.Parameters)
                        {
                            string name = parameter.ParameterName.ToTypeScriptCasing(CurrentContext.Section.TypeScriptUseCamelCasing);
                            code.WriteCode($"[(ngModel)]={name}");
                        }
                    }
                }
                code.WriteLine();
            }

            return code.ToString();
        }
    }
}
