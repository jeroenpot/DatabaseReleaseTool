using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Connection;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class ConnectionFactoryTests
    {
        #region Fields

        private readonly IDatabaseConnectionFactory databaseConnectionFactory = new DatabaseConnectionFactory();

        #endregion

        #region Public Methods and Operators

        [Test]
        public void ShouldCreateMySqlConnectionWhenDatabaseTypeIsMySql()
        {
            IDbConnection connection = databaseConnectionFactory.CreateDatabaseConnection(string.Empty, DatabaseType.MySql);

            Assert.That(connection.GetType().Name, Is.EqualTo("MySqlConnection"));
        }

        [Test]
        public void ShouldCreateOracleConnectionWhenDatabaseTypeIsOracle()
        {
            IDbConnection connection = databaseConnectionFactory.CreateDatabaseConnection(string.Empty, DatabaseType.Oracle);

#pragma warning disable 618
            Assert.That(connection, Is.TypeOf(typeof(OracleConnection)));
#pragma warning restore 618
        }

        [Test]
        public void ShouldCreateSqlConnectionWhenDatabaseTypeIsMsSql()
        {
            IDbConnection connection = databaseConnectionFactory.CreateDatabaseConnection(string.Empty, DatabaseType.MsSql);

            Assert.That(connection, Is.TypeOf(typeof(SqlConnection)));
        }

        #endregion
    }
}