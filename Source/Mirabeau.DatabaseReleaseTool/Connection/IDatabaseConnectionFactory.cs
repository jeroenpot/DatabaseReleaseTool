using System.Data;

using Mirabeau.DatabaseReleaseTool.Arguments;

namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public interface IDatabaseConnectionFactory
    {
        #region Public Methods and Operators

        IDbConnection CreateDatabaseConnection(string connectionString, DatabaseType databaseType);

        #endregion
    }
}