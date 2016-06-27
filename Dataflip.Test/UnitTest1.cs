using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;

using System.Data.Common;

namespace Dataflip.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataflipSettings settings = new DataflipSettings();
            settings.ConfigureCreateCommand( 
                (connnection) => connnection.CreateCommand()
            );
            settings.ConfigureCreateConnection(
                () => new SqlConnection()
            );
            
            var query = new SqlQuery(connectionString,~ )

            var results = query.ExecuteObjectArray(
                "HelloWorldSproc",
                parameters: _ => {
                },
                mapping1: reader => new {
                    Id = reader["Id"] as string
                },
                mapping2: reader => new {
                    Name = reader["Hello World"] as string
                }
            );
            
        }
    }
}
