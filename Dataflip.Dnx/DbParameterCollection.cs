using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip
{
    public static class DbParameterCollectionExtension
    {
        public static DbParameter AddWithNullableValue(
            this DbParameterCollection collection,
            string parameterName,
            DbType type,
            object value)
        {
            DbParameter parameter =
                collection is SqlParameterCollection ?
                    (DbParameter)new SqlParameter() :

                null;

            if (parameter == null)
                throw new InvalidOperationException("Operation not supported for the current parameter collection type.");

            parameter.ParameterName = parameterName;
            parameter.DbType = type;
            parameter.Value = value ?? DBNull.Value;

            collection.Add(parameter);

            return parameter;
        }

        public static DbParameter AddWithValue(
            this DbParameterCollection collection,
            string parameterName,
            object value)
        {
            DbParameter parameter =
                collection is SqlParameterCollection ?
                    (DbParameter)new SqlParameter() :
                null;

            if (parameter == null)
                throw new InvalidOperationException("Operation not supported for the current parameter collection type.");

            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            collection.Add(parameter);

            return parameter;
        }
    }
}