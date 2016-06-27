using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dataflip
{
    public class ResultSet<T1, T2>
    {
        public IEnumerable<T1> Result1 { get; set; }
        public IEnumerable<T2> Result2 { get; set; }
    }

    public class ResultSet<T1, T2, T3>
    {
        public IEnumerable<T1> Result1 { get; set; }
        public IEnumerable<T2> Result2 { get; set; }
        public IEnumerable<T3> Result3 { get; set; }
    }

    public class ResultSet<T1, T2, T3, T4>
    {
        public IEnumerable<T1> Result1 { get; set; }
        public IEnumerable<T2> Result2 { get; set; }
        public IEnumerable<T3> Result3 { get; set; }
        public IEnumerable<T4> Result4 { get; set; }
    }
}
