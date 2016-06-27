using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dataflip
{
    public class QueryMappingException : Exception
    {
        public QueryMappingException(Exception ex)
            :
            base("There was a problem while mapping the values in the query.", ex)
        { }
    }

}
