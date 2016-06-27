using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Dataflip
{
    public class DataflipSettings
    {
        public string ConnectionString { get; set; }
        public int    CommandTimeout { get; set; }

        public DataflipSettings()
        { }

        /// <summary>
        /// Creates a new DataSettings object from a connection string and a command timeout in seconds. Yes, someone
        /// finally thought about command timeouts, finally.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to the database.</param>
        /// <param name="commandTimeout">The connection timeout that the method is willing to wait before it throws a timeout exception.</param>
        public DataflipSettings(string connectionString, int commandTimeout = 10)
        {
            CommandTimeout = commandTimeout;
            ConnectionString = connectionString;
        }

        internal Func<DbConnection, DbCommand> CommandConfiguration = null;
        internal Func<DbConnection> ConnectionConfiguration = null;

        public void ConfigureCreateCommand(Func<DbConnection, DbCommand> commandConfiguration)
        {
            CommandConfiguration = commandConfiguration;
        }

        public void ConfigureCreateConnection(Func<DbConnection> connectionConfiguration)
        {
            ConnectionConfiguration = connectionConfiguration;
        }
    }
}
