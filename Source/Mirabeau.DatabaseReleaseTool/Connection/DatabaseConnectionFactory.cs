using System;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;

using Mirabeau.DatabaseReleaseTool.Arguments;

using MySql.Data.MySqlClient;

namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        #region Public Methods and Operators

        public IDbConnection CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlConnection(connectionString);
                case DatabaseType.Oracle:
#pragma warning disable 618
                    return new OracleConnection(connectionString);
#pragma warning restore 618
                case DatabaseType.MySql:
                    return new MySqlConnection(connectionString);

                default:
                    throw new ArgumentOutOfRangeException(
                        "databaseType", 
                        string.Format(@"No valid database type given '{0}'", databaseType));
            }
        }

        #endregion
    }
}