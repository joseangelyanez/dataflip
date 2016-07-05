using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataflip.Cli.ComponentModel
{
    public static class StringExtensions
    {
        public static string ToTypeScriptCasing(this string s, bool useCamelCasing)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            s = s.Replace("@", "");
            if (s.Length == 0)
                return s;

            return
                useCamelCasing ?
                    (
                        s[0].ToString().ToLower()
                        +
                        s.Substring(1)
                    )
                    :
                    s;
        }
    }
}
