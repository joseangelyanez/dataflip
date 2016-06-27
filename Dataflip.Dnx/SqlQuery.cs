using System.Data.Common;
using System.Data.SqlClient;

namespace Dataflip
{
    /// <summary>
    /// Represents a set of rad method that will query the database in a consitent and uniform way.
    /// </summary>
    public class SqlQuery : DbQuery
    {
        /// <summary>
        /// Creates a new SqlQuery using a DataSettings object. In case you feel like customizing things, DataSettings 
        /// objects are actually cool, they allow you to dictate how a connection and command objects get created. 
        /// </summary>
        /// <param name="settings">
        /// A DataSettings object containing configuration information that defined how the query is
        /// executed.
        /// </param>
        public SqlQuery(DataflipSettings settings)
            :
            base(settings)
        { }

        /// <summary>
        /// Creates a new SqlQuery using a connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string used by the connection.
        /// </param>
        /// <param name="commandTimeout">The time in seconds the method will wait before it throws a timeout exception, we know what you're thinking, "Finally someone makes me think about the timeout of a query before I even setup the connection string", well, hopefully your app will be fast and you won't need this but yeah, here's where you set it.</param>
        public SqlQuery(string connectionString, int commandTimeout = 10)
            :
            base(connectionString, commandTimeout)
        { }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
