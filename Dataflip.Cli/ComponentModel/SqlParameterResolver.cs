using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    /// <summary>
    /// Resolves a SqlParameter information from the information set in the dataflip.json file.
    /// This class also parses the parameter expression contained in the "testParam" section.
    /// </summary>
    public class SqlParameterResolver
    {
        /// <summary>
        /// Represents the mapping between a SqlParameter object and its corresponding CLR type.
        /// </summary>
        public class SqlParameterMapping
        {
            public SqlParameter Parameter { get; set; }
            public Type ClrType { get; set; }
        }

        /// <summary>
        /// Represents the mapping between a CLR Type and a SqlDbType.
        /// </summary>
        public class TypeMapping {
            public SqlDbType SqlType { get; set; }
            public Type      ClrType { get; set; }

            public TypeMapping(SqlDbType sqlType, Type clrType)
            {
                SqlType = sqlType;
                ClrType = clrType;
            }
        }

        /// <summary>
        /// Gets a type map with "type expressions" and TypeMappings.
        /// </summary>
        public readonly static Dictionary<string, TypeMapping> TypeMap =
            new Dictionary<string, TypeMapping>
            {
                { "int64",          new TypeMapping(SqlDbType.BigInt, typeof(Int64)) },
                { "byte[]",         new TypeMapping(SqlDbType.Binary, typeof(Byte[])) },
                { "boolean",        new TypeMapping(SqlDbType.Bit, typeof(Boolean)) },
                { "bool",           new TypeMapping(SqlDbType.Bit, typeof(Boolean)) },
                { "bit",            new TypeMapping(SqlDbType.Bit, typeof(Boolean)) },
                { "string",         new TypeMapping(SqlDbType.Char, typeof(String)) },
                { "varchar",        new TypeMapping(SqlDbType.Char, typeof(String)) },
                { "text",           new TypeMapping(SqlDbType.Char, typeof(String)) },
                { "datetime",       new TypeMapping(SqlDbType.DateTime, typeof(DateTime)) },
                { "datetimeoffset", new TypeMapping(SqlDbType.DateTimeOffset, typeof(DateTimeOffset)) },
                { "decimal",        new TypeMapping(SqlDbType.Decimal, typeof(Decimal)) },
                { "numeric",        new TypeMapping(SqlDbType.Decimal, typeof(Decimal)) },
                { "number",         new TypeMapping(SqlDbType.Decimal, typeof(Decimal)) },
                { "integer",        new TypeMapping(SqlDbType.Int, typeof(Int32)) },
                { "float",          new TypeMapping(SqlDbType.Float, typeof(Double)) },
                { "int",            new TypeMapping(SqlDbType.Int, typeof(Int32)) },
                { "int32",          new TypeMapping(SqlDbType.Int, typeof(Int32)) },
                { "single",         new TypeMapping(SqlDbType.Real, typeof(Single)) },
                { "int16",          new TypeMapping(SqlDbType.SmallInt, typeof(Int16)) },
                { "timespan",       new TypeMapping(SqlDbType.Time, typeof(TimeSpan)) },
                { "byte",           new TypeMapping(SqlDbType.TinyInt, typeof(Byte)) },
                { "guid",           new TypeMapping(SqlDbType.UniqueIdentifier, typeof(Guid)) },
                { "xml",            new TypeMapping(SqlDbType.Xml, typeof(string)) },
            };
         
        /// <summary>
        /// Gets a TypeMapping from a SqlServer friendly type name.
        /// </summary>
        /// <param name="type">A SqlServer friendly type name.</param>
        /// <returns>A TypeMapping with the valid type information.</returns>
        public static TypeMapping GetTypeFromSqlServerFriendlyType(string type)
        {
            if (!TypeMap.ContainsKey(type))
                throw new DataflipToolException($"Couldn't resolve SQL Server/CLR type mapping for {type}");
            return TypeMap[type];
        }

        /// <summary>
        /// Resolves a SqlParameterMapping object from a parameter name and a parameter type expression like "double : 0"
        /// meaning the type is "Double" and the sproc should be tested passing "0" to that sproc.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="parameterTypeExpression">The parameter type expression.</param>
        /// <returns></returns>
        public static SqlParameterMapping Resolve(string parameterName, string parameterTypeExpression)
        {
            var parts = parameterTypeExpression.Replace(" ", "").Split(':');
            string parameterValue = "";
            string parameterType = "";

            if (parts.Length == 1)
            {
                parameterValue = parts[0];
                parameterType = "string";
            }
            if (parts.Length == 2)
            {
                parameterValue = parts[0];
                parameterType = parts[1];
            }


            SqlParameter param = new SqlParameter();


            var typeDef = GetTypeFromSqlServerFriendlyType(parameterType);

            param.ParameterName = parameterName;
            param.SqlDbType = typeDef.SqlType;
            param.Direction = ParameterDirection.Input;

            try
            {
                param.Value = Convert.ChangeType(parameterValue, typeDef.ClrType);
            }
            catch
            {
                throw new DataflipToolException($"There was a problem converting the value '{parameterValue}' to {typeDef.ClrType}.");
            }
            

            return new SqlParameterMapping() { Parameter = param, ClrType = typeDef.ClrType };
        }
    }
}
