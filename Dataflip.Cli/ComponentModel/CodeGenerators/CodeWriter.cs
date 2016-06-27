using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel.CodeGenerators
{
    /// <summary>
    /// Represents a convinient writer to write code.
    /// </summary>
    public class CodeWriter : StringWriter
    {
        /// <summary>
        /// The indentation position.
        /// </summary>
        private int Indentation { get; set; }

        /// <summary>
        /// Increases the indentation.
        /// </summary>
        public void BeingIndentation() {
            Indentation += 1;
        }
        /// <summary>
        /// Decreases the indentation.
        /// </summary>
        public void EndIndentation()
        {
            if (Indentation == 0)
                throw new CodeException("Invalid indentation end, it looks like there's a problem with the code.");

            Indentation -= 1;
        }

        /// <summary>
        /// Writes indented code.
        /// </summary>
        /// <param name="code">The code to write in the line.</param>
        /// <param name="args">The arguments being passed to string.Format within this method.</param>
        public void WriteCode(string code, params string[] args)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            code = args.Length == 0 ?
                    code
                    :
                    string.Format(code, args);

            if (code.StartsWith("}") && !code.StartsWith("{"))
                EndIndentation();

            WriteLine("{0}{1}",
                new String('\t', Indentation),
                code
            );

            if (code.StartsWith("{") && !code.EndsWith("}"))
                BeingIndentation();
        }
    }
}
