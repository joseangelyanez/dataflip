using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataflip.Cli.ComponentModel
{
    public class ContextBuilder
    {
        /// <summary>
        /// Gets or sets the configuration section that this context is based upon.
        /// </summary>
        public ContextSection Configuration { get; set; }

        /// <summary>
        /// Creates a new Context.
        /// </summary>
        /// <param name="configuration">The configuration section.</param>
        public ContextBuilder(ContextSection configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            Configuration = configuration;
        }

        /// <summary>
        /// Creates a new Context object based on the current context configuration.
        /// </summary>
        /// <returns>A new Context object.</returns>
        public Context BuildContext()
        {
            Context context = new Context(Configuration);
            context.Namespace = Configuration.Namespace;
            context.Name = Configuration.Name;

            /* Resolves all the sprocs set in the context configuration. */
            foreach (var sproc in Configuration.Sprocs)
            {
                SprocMethod method = null;

                if (sproc.TestParams.Count > 0)
                    method = ReadSectionAndGenerateSprocMethodUsingTestParams(sproc);
                else
                    method = ReadSectionAndGenerateSprocMethodWithoutTestParams(sproc);

                GlobalProgress.NotifyProgress($"Running sp_help '{sproc.Name}' to derive the parameters.");
                method.Parameters = new SprocParameterResolver(Configuration).Resolve(method.SprocName);
                GlobalProgress.NotifyProgress($"Found {method.Parameters.Count()} parameter(s) for '{sproc.Name}'.");

                context.Methods.Add(method);
                GlobalProgress.NotifyProgress("Done.");
            }

            return context;
        }

        /// <summary>
        /// Creates a SprocMethod object that doesn't require to be tested against to have automatically generated code.
        /// </summary>
        /// <param name="section">The sproc section that defines the sproc configuration.</param>
        /// <returns>A SprocMethod object with the required information for automatic code generation.</returns>
        public SprocMethod ReadSectionAndGenerateSprocMethodWithoutTestParams(SprocSection section)
        {
            GlobalProgress.NotifyProgress(
                new GlobalProgressNotification()
                {
                    Text = section.Name,
                    Color = ConsoleColor.Cyan
                }
            );

            SprocMethod method = new SprocMethod();
            method.SprocName = section.Name;
            method.MethodName = section.Method;
            method.Comments = section.Comments;

            if (section.Return == null)
            {
                method.ReturnType = SprocMethod.ReturnTypes.IntWithRecordsAffected;
            }
            else
            {
                method.ReturnType = SprocMethod.ReturnTypes.ScalarValue;
                method.Return = SqlParameterResolver.GetTypeFromSqlServerFriendlyType(section.Return).ClrType;
            }
            return method;
        }

        /// <summary>
        /// Creates a SprocMethod object that requires to be tested against to define the results.
        /// </summary>
        /// <param name="section">The sproc section that defines the sproc configuration.</param>
        /// <returns>A SprocMethod object with the required information for automatic code generation.</returns>
        public SprocMethod ReadSectionAndGenerateSprocMethodUsingTestParams(SprocSection section)
        {
            GlobalProgress.NotifyProgress(
                new GlobalProgressNotification()
                {
                    Text = $"{section.Name} (testing...)",
                    Color = ConsoleColor.Cyan
                }
            );

            SprocMethod method = new SprocMethod();
            method.MethodName = section.Method;
            method.SprocName = section.Name;
            method.Comments = section.Comments;

            /* Creates a new connection to test the sproc. */
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Configuration.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = section.Name;

                    foreach (var param in section.TestParams)
                    {
                        var sqlParamMapping = SqlParameterResolver.Resolve(param.Name, param.Value);
                        command.Parameters.Add(sqlParamMapping.Parameter);
                    }
                    
                    GlobalProgress.NotifyProgress($"Testing sproc '{section.Name}' with {command.Parameters.Count} parameter(s) ...");
                    /* Tests the sproc. */
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.FieldCount > 1)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                SprocResultProperty sprocProperty = new SprocResultProperty();
                                sprocProperty.ClrType = reader.GetFieldType(i);
                                sprocProperty.PropertyName = reader.GetName(i);

                                method.ResultProperties.Add(sprocProperty);
                                method.ReturnType = SprocMethod.ReturnTypes.IEnumerableOf;
                            }
                            GlobalProgress.NotifyProgress($"'{section.Name}' returned multiple columns, generating result class.");
                        }

                        if (reader.FieldCount == 1)
                        {
                            GlobalProgress.NotifyProgress($"'{section.Name}' returned one column, generating method as scalar.");
                            method.Return = reader.GetFieldType(0);
                            method.ReturnType = SprocMethod.ReturnTypes.ScalarValue;
                        }

                        if (reader.FieldCount == 0)
                        {
                            GlobalProgress.NotifyProgress($"'{section.Name}' didn't return any columns, returning System.Int32.");
                            method.ReturnType = SprocMethod.ReturnTypes.IntWithRecordsAffected;
                        }
                    }
                }
            }

            return method;
        }
    }
}
