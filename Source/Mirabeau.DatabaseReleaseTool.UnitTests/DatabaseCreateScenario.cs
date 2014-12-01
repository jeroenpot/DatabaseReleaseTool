using System;
using System.Data;
using System.IO;
using System.Text;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Connection;
using Mirabeau.DatabaseReleaseTool.Logging;
using Mirabeau.DatabaseReleaseTool.Policies;

using NUnit.Framework;

using Rhino.Mocks;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class DatabaseCreateScenario
    {
        #region Public Methods and Operators

        [Test]
        public void ShouldCallExecuteAllScriptsForDatabaseWhenExecuteAllScriptsForDatabaseIsNotCalled()
        {
            // arrange
            DatabaseReleaseTool databaseReleaseToolMock =
                MockRepository.GeneratePartialMock<DatabaseReleaseTool>(
                    Environment.CurrentDirectory, 
                    new CreateDatabasePolicyComposite(), 
                    new FileStructurePolicyComposite(), 
                    new ConsoleOutputLogger(), 
                    new ConnectionStringFactory(), 
                    null);
            DatabaseConnectionParameters databaseConnectionParameters = new DatabaseConnectionParameters
                                                                        {
                                                                            BeforeExecuteScriptsAction =
                                                                                BeforeExecuteScriptsAction
                                                                                .None
                                                                        };
            databaseReleaseToolMock.Expect(
                x => x.ExecuteAllScriptsForDatabase("1.0.0", "2.0.0", databaseConnectionParameters, Encoding.ASCII))
                .Repeat.Once()
                .Return(new ExcecutionResult());

            // act
            databaseReleaseToolMock.Execute("1.0.0", "2.0.0", databaseConnectionParameters, Encoding.ASCII);

            // assert
            databaseReleaseToolMock.VerifyAllExpectations();
        }

        [Test]
        public void
            ShouldCallExecuteCreateDatabaseAndExecuteAllScriptsForDatabaseWhenBeforeExecuteScriptsActionIsCreateDatabaseAndANdResultOfExecuteCreateDatabaseIsSucces()
        {
            // arrange
            DatabaseReleaseTool databaseReleaseToolMock =
                MockRepository.GeneratePartialMock<DatabaseReleaseTool>(
                    Environment.CurrentDirectory, 
                    new CreateDatabasePolicyComposite(), 
                    new FileStructurePolicyComposite(), 
                    new ConsoleOutputLogger(), 
                    new ConnectionStringFactory(), 
                    null);
            DatabaseConnectionParameters databaseConnectionParameters = new DatabaseConnectionParameters
                                                                        {
                                                                            BeforeExecuteScriptsAction =
                                                                                BeforeExecuteScriptsAction
                                                                                .CreateDatabase
                                                                        };
            databaseReleaseToolMock.Expect(x => x.ExecuteCreateDatabase(databaseConnectionParameters, Encoding.UTF7))
                .Repeat.Once()
                .Return(new ExcecutionResult { Success = true });
            databaseReleaseToolMock.Expect(
                x => x.ExecuteAllScriptsForDatabase("1.0.0", "2.0.0", databaseConnectionParameters, Encoding.UTF7))
                .Repeat.Once()
                .Return(new ExcecutionResult());

            // act
            databaseReleaseToolMock.Execute("1.0.0", "2.0.0", databaseConnectionParameters, Encoding.UTF7);

            // assert
            databaseReleaseToolMock.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCallExecuteCreateDatabaseWhenBeforeExecuteScriptsActionIsCreateDatabase()
        {
            // arrange
            DatabaseReleaseTool databaseReleaseToolMock =
                MockRepository.GeneratePartialMock<DatabaseReleaseTool>(
                    Environment.CurrentDirectory, 
                    new CreateDatabasePolicyComposite(), 
                    new FileStructurePolicyComposite(), 
                    new ConsoleOutputLogger(), 
                    new ConnectionStringFactory(), 
                    null);
            DatabaseConnectionParameters databaseConnectionParameters = new DatabaseConnectionParameters
                                                                        {
                                                                            BeforeExecuteScriptsAction =
                                                                                BeforeExecuteScriptsAction
                                                                                .CreateDatabase
                                                                        };
            databaseReleaseToolMock.Expect(x => x.ExecuteCreateDatabase(databaseConnectionParameters, Encoding.ASCII))
                .Repeat.Once()
                .Return(new ExcecutionResult());

            // act
            databaseReleaseToolMock.Execute(string.Empty, string.Empty, databaseConnectionParameters, Encoding.ASCII);

            // assert
            databaseReleaseToolMock.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnTrueWhenNoCreateDatabaseScriptIsGiven()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldReturnTrueWhenNoCreateDatabaseScriptIsGiven");

            IDatabaseConnectionFactory databaseConnectionFactory = MockRepository.GenerateMock<IDatabaseConnectionFactory>();
            IDbConnection connection = MockRepository.GenerateStub<IDbConnection>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();
            databaseConnectionFactory.Expect(
                c => c.CreateDatabaseConnection("Data Source=.;Initial Catalog=master;User ID=testUser;Password=testPassword", DatabaseType.MsSql))
                .Repeat.Once()
                .Return(connection);
            connection.Expect(method => method.BeginTransaction()).Return(transaction);
            connection.Expect(method => method.CreateCommand()).Return(command);

            DatabaseReleaseTool dbreleaseTool = new DatabaseReleaseTool(
                directoryInfo.FullName, 
                new CreateDatabasePolicyComposite(), 
                null, 
                new ConsoleOutputLogger(), 
                new ConnectionStringFactory(), 
                databaseConnectionFactory);

            DatabaseConnectionParameters parameters = new DatabaseConnectionParameters();
            parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            parameters.DatabaseType = DatabaseType.MsSql;
            parameters.CreationType = ConnectionStringCreationType.FromArguments;
            parameters.Arguments.Hostname = ".";
            parameters.Arguments.Username = "testUser";
            parameters.Arguments.Password = "testPassword";
            parameters.Arguments.Database = "testdb";

            // Act
            ExcecutionResult result = dbreleaseTool.ExecuteCreateDatabase(parameters, Encoding.Default);

            // Assert
            Assert.That(result.Success, Is.True, "error: {0} ", result.Errors);
        }

        [Test]
        public void ShouldReturnTrueWhenNoScriptIsExecuted()
        {
            // Setup
            DirectoryInfo directoryInfo = DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldReturnTrueWhenNoScriptIsExecuted");
            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "1.1.0_AnyFile.sql"));

            IDatabaseConnectionFactory databaseConnectionFactory = MockRepository.GenerateMock<IDatabaseConnectionFactory>();
            IDbConnection connection = MockRepository.GenerateStub<IDbConnection>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();
            databaseConnectionFactory.Expect(
                c => c.CreateDatabaseConnection("Data Source=.;Initial Catalog=master;User ID=testUser;Password=testPassword", DatabaseType.MsSql))
                .Repeat.Once()
                .Return(connection);
            connection.Expect(method => method.BeginTransaction()).Return(transaction);
            connection.Expect(method => method.CreateCommand()).Return(command);

            DatabaseReleaseTool dbreleaseTool = new DatabaseReleaseTool(
                directoryInfo.FullName, 
                new CreateDatabasePolicyComposite(), 
                null, 
                new ConsoleOutputLogger(), 
                new ConnectionStringFactory(), 
                databaseConnectionFactory);

            DatabaseConnectionParameters parameters = new DatabaseConnectionParameters();
            parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            parameters.DatabaseType = DatabaseType.MsSql;
            parameters.CreationType = ConnectionStringCreationType.FromArguments;
            parameters.Arguments.Hostname = ".";
            parameters.Arguments.Username = "testUser";
            parameters.Arguments.Password = "testPassword";
            parameters.Arguments.Database = "testdb";

            // Act
            ExcecutionResult result = dbreleaseTool.ExecuteCreateDatabase(parameters, Encoding.Default);

            // Assert
            Assert.That(result.Success, Is.True, "error: {0} ", result.Errors);
        }

        [Test]
        public void ShouldUseDbConnectionToMasterWhenBeforeExecuteScriptsActionIsCreateDatabase()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure(
                    "ShouldUseDbConnectionToMasterWhenBeforeExecuteScriptsActionIsCreateDatabase");
            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "CreateDatabase.sql"));

            IDatabaseConnectionFactory databaseConnectionFactory = MockRepository.GenerateMock<IDatabaseConnectionFactory>();
            IDbConnection connection = MockRepository.GenerateStub<IDbConnection>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();
            databaseConnectionFactory.Expect(
                c => c.CreateDatabaseConnection("Data Source=.;Initial Catalog=master;User ID=testUser;Password=testPassword", DatabaseType.MsSql))
                .Repeat.Once()
                .Return(connection);
            connection.Expect(method => method.BeginTransaction()).Return(transaction);
            connection.Expect(method => method.CreateCommand()).Return(command);

            DatabaseReleaseTool dbreleaseTool = new DatabaseReleaseTool(
                directoryInfo.FullName, 
                new CreateDatabasePolicyComposite(), 
                null, 
                new ConsoleOutputLogger(), 
                new ConnectionStringFactory(), 
                databaseConnectionFactory);

            DatabaseConnectionParameters parameters = new DatabaseConnectionParameters
                                                      {
                                                          BeforeExecuteScriptsAction =
                                                              BeforeExecuteScriptsAction.CreateDatabase, 
                                                          DatabaseType = DatabaseType.MsSql, 
                                                          CreationType =
                                                              ConnectionStringCreationType.FromArguments, 
                                                          Arguments =
                                                          {
                                                              Hostname = ".", 
                                                              Username = "testUser",
                                                              Password = "testPassword", 
                                                              Database = "FruitDB"
                                                          }
                                                      };

            // Act
            dbreleaseTool.ExecuteCreateDatabase(parameters, Encoding.Default);

            // Assert
            databaseConnectionFactory.VerifyAllExpectations();
        }

        [Test]
        public void SmokeTest()
        {
            // Setup
            DirectoryInfo directoryInfo = DirectoryStructureHelper.CreateValidDatabaseDirStructure("SmokeTest");
            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "CreateDatabase.sql"));

            IDatabaseConnectionFactory databaseConnectionFactory = MockRepository.GenerateMock<IDatabaseConnectionFactory>();
            IDbConnection connection = MockRepository.GenerateStub<IDbConnection>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();
            databaseConnectionFactory.Expect(
                c => c.CreateDatabaseConnection("Data Source=.;Initial Catalog=master;User ID=testUser;Password=testPassword", DatabaseType.MsSql))
                .Repeat.Once()
                .Return(connection);
            connection.Expect(method => method.BeginTransaction()).Return(transaction);
            connection.Expect(method => method.CreateCommand()).Return(command);

            DatabaseReleaseTool dbreleaseTool = new DatabaseReleaseTool(
                directoryInfo.FullName, 
                new CreateDatabasePolicyComposite(), 
                null, 
                new ConsoleOutputLogger(), 
                new ConnectionStringFactory(), 
                databaseConnectionFactory);

            DatabaseConnectionParameters parameters = new DatabaseConnectionParameters();
            parameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.CreateDatabase;
            parameters.DatabaseType = DatabaseType.MsSql;
            parameters.CreationType = ConnectionStringCreationType.FromArguments;
            parameters.Arguments.Hostname = ".";
            parameters.Arguments.Username = "testUser";
            parameters.Arguments.Password = "testPassword";
            parameters.Arguments.Database = "testdb";

            // Act
            ExcecutionResult result = dbreleaseTool.ExecuteCreateDatabase(parameters, Encoding.Default);

            // Assert
            Assert.That(result.Success, Is.True, "error: {0} ", result.Errors);
        }

        #endregion
    }
}