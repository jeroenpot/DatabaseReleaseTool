using System.Configuration;
using System.IO;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Connection;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class ConnectionStringFactoryTests
    {
        #region Fields

        private readonly IConnectionStringFactory _connectionStringFactory = new ConnectionStringFactory();

        private DatabaseConnectionParameters _parameters;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void SetUp()
        {
            _parameters = CreateTestParameters();
        }

        [Test]
        public void ShouldCreateConnectionStringFromConfigurationFile()
        {
            // Arrange            
            DatabaseConnectionParameters fromFileParameters = new DatabaseConnectionParameters
                                                              {
                                                                  ConfigurationFileName = "TestConfig.config", 
                                                                  CreationType =
                                                                      ConnectionStringCreationType
                                                                      .FromConfigurationFile
                                                              };

            // Act
            string connectionString = _connectionStringFactory.Create(fromFileParameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("Data Source=.;Initial Catalog=testdb;User ID=xx;Password=xxxx"));
        }

        [Test]
        public void ShouldCreateMsSqlConnectionStringFromArgumentParameters()
        {
            // Arrange                        
            _parameters.DatabaseType = DatabaseType.MsSql;

            // Act
            string connectionString = _connectionStringFactory.Create(_parameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("Data Source=.;Initial Catalog=testdb;User ID=xx;Password=xxxx"));
        }

        [Test]
        public void ShouldCreateMySqlConnectionStringFromArgumentParameters()
        {
            // Arrange                        
            _parameters.DatabaseType = DatabaseType.MySql;

            // Act
            string connectionString = _connectionStringFactory.Create(_parameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("server=.;user id=xx;password=xxxx;database=testdb"));
        }

        [Test]
        public void ShouldCreateOracleConnectionStringFromArgumentParameters()
        {
            // Arrange                        
            _parameters.DatabaseType = DatabaseType.Oracle;

            // Act
            string connectionString = _connectionStringFactory.Create(_parameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("Data Source=.;User ID=xx;Password=xxxx"));
        }

        [Test]
        public void
            ShouldReplaceInitialCatalogWithMasterWhenConnectionStringCreationTypeIsFromArgumentsAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeMsSql()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromArguments;
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.MsSql;

            // Act
            string connectionString = _connectionStringFactory.Create(_parameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("Data Source=.;Initial Catalog=master;User ID=xx;Password=xxxx"));
        }

        [Test]
        public void
            ShouldReplaceInitialCatalogWithMasterWhenConnectionStringCreationTypeIsFromConfigurationFileAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeMsSql()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromConfigurationFile;
            _parameters.ConfigurationFileName = "TestConfigWithCreateDatabase.config";
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.MsSql;

            // Act
            string connectionString = _connectionStringFactory.Create(_parameters);

            // Assert
            Assert.That(connectionString, Is.EqualTo("Data Source=.;Initial Catalog=master;User ID=xx;Password=xxxx"));
        }

        [Test]
        public void
            ShouldThrowConfigurationErrorsExceptionWhenConnectionStringCreationTypeIsFromArgumentsAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeIsMySql()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromArguments;
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.MySql;

            Assert.That(
                () => { _connectionStringFactory.Create(_parameters); }, 
                Throws.TypeOf(typeof(ConfigurationErrorsException)).And.Message.StringContaining("MySql"));
        }

        [Test]
        public void
            ShouldThrowConfigurationErrorsExceptionWhenConnectionStringCreationTypeIsFromArgumentsAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeIsOracle()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromArguments;
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.Oracle;

            Assert.That(
                () => { _connectionStringFactory.Create(_parameters); }, 
                Throws.TypeOf(typeof(ConfigurationErrorsException)).And.Message.StringContaining("Oracle"));
        }

        [Test]
        public void
            ShouldThrowConfigurationErrorsExceptionWhenConnectionStringCreationTypeIsFromConfigurationFileAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeIsMySql()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromConfigurationFile;
            _parameters.ConfigurationFileName = "TestConfigWithCreateDatabase.config";
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.MySql;

            Assert.That(
                () => { _connectionStringFactory.Create(_parameters); }, 
                Throws.TypeOf(typeof(ConfigurationErrorsException)).And.Message.StringContaining("MySql"));
        }

        [Test]
        public void
            ShouldThrowConfigurationErrorsExceptionWhenConnectionStringCreationTypeIsFromConfigurationFileAndBeforeExecuteScriptsActionIsCreateDatabaseAndDatabaseTypeIsOracle()
        {
            // Arrange
            _parameters.CreationType = ConnectionStringCreationType.FromConfigurationFile;
            _parameters.ConfigurationFileName = "TestConfigWithCreateDatabase.config";
            _parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            _parameters.DatabaseType = DatabaseType.Oracle;

            Assert.That(
                () => { _connectionStringFactory.Create(_parameters); }, 
                Throws.TypeOf(typeof(ConfigurationErrorsException)).And.Message.StringContaining("Oracle"));
        }

        [Test]
        public void ShouldThrowFileNotFoundExceptionWhenParameterSpecifiesConfigFileThatDoesNotExist()
        {
            DatabaseConnectionParameters fromFileParameters = new DatabaseConnectionParameters
                                                              {
                                                                  ConfigurationFileName =
                                                                      "IDoNoTExist.config", 
                                                                  CreationType =
                                                                      ConnectionStringCreationType
                                                                      .FromConfigurationFile
                                                              };
            Assert.That(() => { _connectionStringFactory.Create(fromFileParameters); }, Throws.TypeOf(typeof(FileNotFoundException)));
        }

        #endregion

        #region Methods

        private static DatabaseConnectionParameters CreateTestParameters()
        {
            return new DatabaseConnectionParameters
                   {
                       CreationType = ConnectionStringCreationType.FromArguments, 
                       DatabaseType = DatabaseType.MsSql, 
                       Arguments =
                       {
                           Hostname = ".", 
                           Username = "xx", 
                           Password = "xxxx", 
                           Database = "testdb"
                       }
                   };
        }

        #endregion
    }
}