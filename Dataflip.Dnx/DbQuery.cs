using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dataflip
{
    /// <summary>
    /// Contains methods that access a Database and execute queries in 
    /// a safely disposed context. The class is abstract use SqlQuery to 
    /// run queries against a Sql Server DB.
    /// </summary>
    public abstract class DbQuery
    {
        public DataflipSettings Settings { get; set; }

        /// <summary>
        /// Creates a new instance of the DbQuery class using a settings object.
        /// </summary>
        /// <param name="settings"></param>
        public DbQuery(DataflipSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Settings = settings;
        }

        /// <summary>
        /// Creates a new instance of the DbQuery object using a connection string,
        /// use the Settings properties to setup additional settings.
        /// </summary>
        /// <param name="connectionString">A valid connection string.</param>
        /// <param name="commandTimeout">The time in seconds for the query to the method to wait before it throws a timeout exception, we know what you're thinking, finally someone makes think about the timeout of a query before I even setup the connection string.</param>
        public DbQuery(string connectionString, int commandTimeout)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            Settings = new DataflipSettings()
            {
                ConnectionString = connectionString,
                CommandTimeout = commandTimeout
            };
        }

        /// <summary>
        /// When overridden by another class it creates a new DbConnection.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual DbConnection CreateConnection(string connectionString)
        {
            throw new InvalidOperationException("CreateConnection must be overriden before executing any query.");
        }
        
        /// <summary>
        /// Creates a new instance of the DbQuery class.
        /// </summary>
        public DbQuery()
        {}

        /// <summary>
        /// Executes a query, iterates through the rows and 
        /// executes a delegate that can be used as a lamda expression
        /// to map the results into an IEnumerable of T.
        /// </summary>
        /// <typeparam name="T">
        /// The resulting type of the IEnumerable being returned by this method.</typeparam>
        /// <param name="connectionStringName">The connection string that will be used to run the query.</param>
        /// <param name="query">The query that will be executed in the Database</param>
        /// <param name="mapping">A delegate that can return an instance of T.</param>
        /// <param name="parameters">A delegate that receives a collection of parameters and 
        /// sets them up with any required parameter to run the query.
        /// </param>
        /// <param name="commandType">
        /// Determines whether this query is run as a stored procedure or text.</param>
        /// <returns>
        /// </returns>
        [DebuggerStepThrough]
        public IEnumerable<T> ExecuteObjectArray<T>(
            string query,
            Func<DbDataReader, T> mapping,
            ParameterSetup parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");
            if (mapping == null)
                throw new ArgumentNullException("mapping");

            List<T> results = new List<T>();

            DbConnection connection = null;
            try
            {
                if (Settings.ConnectionConfiguration != null)
                    connection = Settings.ConnectionConfiguration();
                else
                    connection = CreateConnection(Settings.ConnectionString);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (DbCommand command =
                    Settings.CommandConfiguration == null ?
                        connection.CreateCommand()
                        :
                        Settings.CommandConfiguration(connection))
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    if (parameters != null)
                        parameters(command.Parameters);

                    try
                    {
                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var result = mapping(reader);

                                results.Add(result);
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new QueryMappingException(ex);
                    }
                }
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }

            return results;
        }

        private static readonly Func<DbDataReader, Boolean> m_empty = null;

        /// <summary>
        /// Executes a Sql Query that returns 2 result sets. Why would you do that? Well, because using multiple resultsets in the same query
        /// can improve your performance since there is no need to reopen a connection or to open
        /// another data reader. Don't worry, we took care of the tough part, both results are return in separate properties in the
        /// ResultSet object.
        /// </summary>
        /// <typeparam name="TResult1">The type for the first result, this method also supports anonymous types.</typeparam>
        /// <typeparam name="TResult2">The type for the second result, this method also supports anonymous types.</typeparam>
        /// <param name="query">The query to run against the database.</param>
        /// <param name="mapping1">A lambda expression with the mapping for the first result.</param>
        /// <param name="mapping2">A lambda expression with the mapping for the second result.</param>
        /// <param name="parameters">A delegate to setup the parameters for the query. 
        /// ie. 
        /// ...
        /// parameters : _ => { _.AddWithValue("@MyParam", paramValue); }
        /// ...
        /// </param>
        /// <param name="commandType">Sets whether the query is run as a Stored Procedure or a Query, this parameter defaults to Stored Procedure - Yes, we did that on purpose.</param>
        /// <returns>
        /// A result set with two properties for each result.
        /// </returns>
        public ResultSet<TResult1, TResult2> ExecuteObjectArray<TResult1, TResult2>(
            string query,
            Func<DbDataReader, TResult1> mapping1,
            Func<DbDataReader, TResult2> mapping2,
            ParameterSetup  parameters  = null,
            CommandType     commandType = CommandType.StoredProcedure)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (mapping1 == null)
                throw new ArgumentNullException("mapping1");
            if (mapping2 == null)
                throw new ArgumentNullException("mapping2");

            var resultSet = ExecuteObjectArray(
                query: query, 
                mapping1: mapping1, 
                mapping2: mapping2, 
                mapping3: m_empty, 
                mapping4: m_empty, 
                parameters : parameters,
                commandType : commandType
            ); 

            return new ResultSet<TResult1, TResult2>()
            {
                Result1 = resultSet.Result1,
                Result2 = resultSet.Result2
            };
        }

        /// <summary>
        /// Execute a Sql Query that returns 3 result sets. Using multiple resultsets in the same query
        /// can improve your performance since there is no need to reopen a connection or to open
        /// another data reader.
        /// </summary>
        /// <typeparam name="TResult1">The type for the first result, this method also supports anonymous types.</typeparam>
        /// <typeparam name="TResult2">The type for the second result, this method also supports anonymous types.</typeparam>
        /// <param name="query">The query to run against the database.</param>
        /// <param name="mapping1">A lambda expression with the mapping for the first result.</param>
        /// <param name="mapping2">A lambda expression with the mapping for the second result.</param>
        /// <param name="mapping3">A lambda expression with the mapping for the third result.</param>
        /// <param name="parameters">A delegate to setup the parameters for the query. 
        /// ie. 
        /// ...
        /// parameters : _ => { _.AddWithValue("@MyParam", paramValue); }
        /// ...
        /// </param>
        /// <param name="commandType">Sets whether the query is run as a Stored Procedure or a Query, this parameter defaults to Stored Procedure - Yes, we did that on purpose.</param>
        /// <returns>
        /// A result set with three properties for each result.
        /// </returns>
        public ResultSet<TResult1, TResult2, TResult3> ExecuteObjectArray<TResult1, TResult2, TResult3>(
            string query,
            Func<DbDataReader, TResult1> mapping1,
            Func<DbDataReader, TResult2> mapping2,
            Func<DbDataReader, TResult3> mapping3,
            ParameterSetup parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (mapping1 == null)
                throw new ArgumentNullException("mapping1");
            if (mapping2 == null)
                throw new ArgumentNullException("mapping2");
            if (mapping3 == null)
                throw new ArgumentNullException("mapping3");

            var resultSet = ExecuteObjectArray(
                query: query,
                mapping1: mapping1,
                mapping2: mapping2,
                mapping3: mapping3,
                mapping4: m_empty,
                parameters: parameters,
                commandType: commandType
            );

            return new ResultSet<TResult1, TResult2, TResult3>()
            {
                Result1 = resultSet.Result1,
                Result2 = resultSet.Result2,
                Result3 = resultSet.Result3
            };
        }

        /// <summary>
        /// Execute a Sql Query that returns 4 result sets. Using multiple resultsets in the same query
        /// can improve your performance since there is no need to reopen a connection or to open
        /// another data reader.
        /// </summary>
        /// <typeparam name="TResult1">The type for the first result, this method also supports anonymous types.</typeparam>
        /// <typeparam name="TResult2">The type for the second result, this method also supports anonymous types.</typeparam>
        /// <param name="query">The query to run against the database.</param>
        /// <param name="mapping1">A lambda expression with the mapping for the first result.</param>
        /// <param name="mapping2">A lambda expression with the mapping for the second result.</param>
        /// <param name="mapping3">A lambda expression with the mapping for the third result.</param>
        /// <param name="mapping4">A lambda expression with the mapping for the fourth result.</param>
        /// <param name="parameters">A delegate to setup the parameters for the query. 
        /// ie. 
        /// ...
        /// parameters : _ => { _.AddWithValue("@MyParam", paramValue); }
        /// ...
        /// </param>
        /// <param name="commandType">Sets whether the query is run as a Stored Procedure or a Query, this parameter defaults to Stored Procedure - Yes, we did that on purpose.</param>
        /// <returns>
        /// A result set with four properties for each result.
        /// </returns>
        [DebuggerStepThrough]
        public ResultSet<TResult1, TResult2, TResult3, TResult4> ExecuteObjectArray<TResult1, TResult2, TResult3, TResult4>(
            string query,
            Func<DbDataReader, TResult1> mapping1,
            Func<DbDataReader, TResult2> mapping2,
            Func<DbDataReader, TResult3> mapping3,
            Func<DbDataReader, TResult4> mapping4,
            ParameterSetup parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (mapping1 == null)
                throw new ArgumentNullException("mapping1");
            if (mapping2 == null)
                throw new ArgumentNullException("mapping2");
            if (mapping3 == null)
                throw new ArgumentNullException("mapping3");
            if (mapping4 == null)
                throw new ArgumentNullException("mapping3");

            List<TResult1> results1 = null;
            List<TResult2> results2 = null;
            List<TResult3> results3 = null;
            List<TResult4> results4 = null;

            DbConnection connection = null;
            try
            {
                if (Settings.ConnectionConfiguration != null)
                    connection = Settings.ConnectionConfiguration();
                else
                    connection = CreateConnection(Settings.ConnectionString);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    if (parameters != null)
                        parameters(command.Parameters);

                    try
                    {
                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            results1 = new List<TResult1>();
                            while (reader.Read())
                            {
                                var result1 = mapping1(reader);
                                results1.Add(result1);
                            }

                            if (reader.NextResult())
                            {
                                if (mapping2 == null)
                                    throw new InvalidOperationException("The query seems to have a second result set but no mappings2 was specified.");

                                results2 = new List<TResult2>();
                                while (reader.Read())
                                {
                                    var result2 = mapping2(reader);
                                    results2.Add(result2);
                                }
                            }

                            if (reader.NextResult())
                            {
                                if (mapping3 == null)
                                    throw new InvalidOperationException("The query seems to have a third result set but no mappings3 was specified.");

                                results3 = new List<TResult3>();
                                while (reader.Read())
                                {
                                    var result3 = mapping3(reader);
                                    results3.Add(result3);
                                }
                            }

                            if (reader.NextResult())
                            {
                                if (mapping2 == null)
                                    throw new InvalidOperationException("The query seems to have a fourth result set but no mappings4 was specified.");

                                results4 = new List<TResult4>();
                                while (reader.Read())
                                {
                                    var result4 = mapping4(reader);
                                    results4.Add(result4);
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new QueryMappingException(ex);
                    }
                }
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }

            return new ResultSet<TResult1, TResult2, TResult3, TResult4>() {
                Result1 = results1,
                Result2 = results2,
                Result3 = results3,
                Result4 = results4
            };
        }


        [DebuggerStepThrough]
        public object ExecuteScalar(
            string query,
            ParameterSetup parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            object result = null;

            DbConnection connection = null;
            try
            {
                if (Settings.ConnectionConfiguration != null)
                    connection = Settings.ConnectionConfiguration();
                else
                    connection = CreateConnection(Settings.ConnectionString);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    if (parameters != null)
                        parameters(command.Parameters);

                    try
                    {
                        result = command.ExecuteScalar();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new QueryMappingException(ex);
                    }
                }
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();   
            }

            return result;
        }

        [DebuggerStepThrough]
        public int ExecuteNonQuery(
            string query,
            ParameterSetup parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            DbConnection connection = null;
            try
            {
                if (Settings.ConnectionConfiguration != null)
                    connection = Settings.ConnectionConfiguration();
                else
                    connection = CreateConnection(Settings.ConnectionString);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    parameters?.Invoke(command.Parameters);

                    try
                    {
                        command.CommandType = commandType;
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new QueryMappingException(ex);
                    }
                }
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }
    }
}

