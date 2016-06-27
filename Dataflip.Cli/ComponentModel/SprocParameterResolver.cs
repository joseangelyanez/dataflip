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
    /// Represents an object that can receive a sproc name and generate a list of SprocParameters objects by
    /// using "sp_help" against the database.
    /// </summary>
    public class SprocParameterResolver
    {
        /// <summary>
        /// Gets a list of type mappings for SQL Server and the CLR.
        /// </summary>
        public static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>
            {
                { "bigint", typeof(Int64) },
                { "binary", typeof(Byte[]) },
                { "bit", typeof(Boolean) },
                { "char", typeof(String) },
                { "date", typeof(DateTime) },
                { "datetime", typeof(DateTime) },
                { "datetime2", typeof(DateTime) },
                { "datetimeoffset", typeof(DateTimeOffset) },
                { "decimal", typeof(Decimal) },
                { "float", typeof(Double) },
                { "int", typeof(Int32) },
                { "money", typeof(Decimal) },
                { "nchar", typeof(String) },
                { "ntext", typeof(String) },
                { "nvarChar", typeof(String) },
                { "real", typeof(Single) },
                { "smallint", typeof(Int16) },
                { "smallmoney", typeof(Decimal) },
                { "structured", typeof(Object) }, // might not be best mapping...
                { "text", typeof(String) },
                { "time", typeof(TimeSpan) },
                { "timestamp", typeof(Byte[]) },
                { "tinyint", typeof(Byte) },
                { "udt", typeof(Object) },  // might not be best mapping...
                { "uniqueidentifier", typeof(Guid) },
                { "varbinary", typeof(Byte[]) },
                { "varchar", typeof(String) },
                { "variant", typeof(Object) },
                { "xml", typeof(String) }
            };

        /// <summary>
        /// Gets or sets the context configuration.
        /// </summary>
        public ContextSection Configuration { get; set; }

        /// <summary>
        /// Gets the CLR type that matches a SQL Server type.
        /// </summary>
        /// <param name="type">A SQL Server type.</param>
        /// <returns>A CLR type.</returns>
        public Type GetClrTypeFromSqlServerType(string type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return TypeMap[type.ToLower()];
        }

        /// <summary>
        /// Constructs a new SprocParameterResolver object.
        /// </summary>
        /// <param name="configuration">Represents the current context configuration.</param>
        public SprocParameterResolver(ContextSection configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Resolves the parameters for a given sproc name.
        /// </summary>
        /// <param name="sprocName">The sproc name.</param>
        /// <returns>An IEnumerable of SproParameters that contains all the parameter information about the sproc.</returns>
        public IEnumerable<SprocParameter> Resolve(string sprocName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = Configuration.ConnectionString;
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"sp_help {sprocName}";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<SprocParameter> parameters = new List<SprocParameter>();

                            reader.NextResult();

                            while (reader.Read())
                            {
                                SprocParameter param = new SprocParameter();

                                param.ClrType = GetClrTypeFromSqlServerType(reader["Type"] as string);
                                param.ParameterName = reader["Parameter_name"] as string;

                                parameters.Add(param);
                            }

                            return parameters;
                        }
                    }
                }
            }
            catch
            {
                GlobalProgress.NotifyProgress("There was a problem running sp_test to get the parameter information.");
                throw;
            }
        }

    }
}
