using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Connection;
using Mirabeau.DatabaseReleaseTool.Logging;
using Mirabeau.DatabaseReleaseTool.Policies;
using Mirabeau.DatabaseReleaseTool.Sql;

using NUnit.Framework;

using Rhino.Mocks;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class DatabaseReleaseToolTest
    {
        #region Fields

        private readonly IConnectionStringFactory _connectionStringFactory = new ConnectionStringFactory();

        #endregion

        #region Public Methods and Operators

        [Test]
        public void AllFilesMustAdhereToPrefixPolicy()
        {
            string databaseScriptsPathName = "AllFilesMustAdhereToPrefixPolicyTest";

            // Setup            
            DirectoryInfo databaseScriptsRoot = DirectoryStructureHelper.CreateValidDatabaseDirStructure(databaseScriptsPathName);

            FilenamePrefixPolicy policy = new FilenamePrefixPolicy();

            string validFilename = "1.0.0.0_0001_CreateTables.sql";
            string invalidFilename = "CreateTables.sql";

            string validFilenameFullname = databaseScriptsRoot.GetDirectories()[2].FullName + "\\" + validFilename;
            string invalidFilenameFullname = databaseScriptsRoot.GetDirectories()[2].FullName + "\\" + invalidFilename;

            // Create valid filename
            DirectoryStructureHelper.CreateEmptyFile(validFilenameFullname);

            PolicyResult result = policy.Check(databaseScriptsRoot);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Messages);

            // Create invalid filename
            DirectoryStructureHelper.CreateEmptyFile(invalidFilenameFullname);

            result = policy.Check(databaseScriptsRoot);

            Assert.IsFalse(result.Success);
            Assert.That(result.Messages.Count, Is.EqualTo(1));
        }

        [Test]
        public void AtLeastOneDirectoryMustContainSqlFiles()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("AtLeastOneDirectoryMustContainSqlFilesTest");
            IFileStructurePolicy policy = new MustContainSqlFilePolicy();

            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "1.0_TestScript.sql"));

            PolicyResult result = policy.Check(directoryInfo);

            // Empty directory. Policy should fail.
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Messages);
        }

        [Test]
        public void AtLeastOneDirectoryMustContainSqlFilesFailsOnEmptyDirs()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("AtLeastOneDirectoryMustContainSqlFilesTest");
            IFileStructurePolicy policy = new MustContainSqlFilePolicy();

            PolicyResult result = policy.Check(directoryInfo);

            // Empty directory. Policy should fail.
            Assert.IsFalse(result.Success);
            Assert.That(result.Messages.Count, Is.EqualTo(1));
        }

        [Test]
        public void AtLeastOneDirectoryMustContainSqlFilesFailsWhenOnlyIgnoredDirectoriesHaveSqlFiles()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("MustContainSqlFilesFailsWhenOnlyIgnoredDirectoriesHaveSqlFiles");
            IFileStructurePolicy policy = new MustContainSqlFilePolicy();

            // Create sql files ONLY in the ignoredirlist
            foreach (string ignoreDir in Resources.DirectoryIgnoreListStructureTemplateList())
            {
                string fullPath = Path.Combine(directoryInfo.FullName, ignoreDir);
                DirectoryStructureHelper.CreateEmptyFile(Path.Combine(fullPath, "1.0.0_IgnoreMe.sql"));
            }

            // Act
            PolicyResult result = policy.Check(directoryInfo);

            // Assert
            // Empty directory. Policy should fail.
            Assert.IsFalse(result.Success);
            Assert.That(result.Messages.Count, Is.EqualTo(1));
        }

        [Test]
        public void DatabaseScriptsPathMustExistException()
        {
            Assert.That(
                () =>
                    new DatabaseReleaseTool(
                        "IDoNotExistPath", 
                        null, 
                        null, 
                        new ConsoleOutputLogger(), 
                        _connectionStringFactory, 
                        CreateMockDatabaseConnectionFactory()), 
                Throws.Exception.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public void DatabaseScriptsPathMustNotBeEmptyException()
        {
            Assert.That(
                () =>
                    new DatabaseReleaseTool(
                        string.Empty, 
                        null, 
                        null, 
                        new ConsoleOutputLogger(), 
                        _connectionStringFactory, 
                        CreateMockDatabaseConnectionFactory()), 
                Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void DirectoryStructurePoliceReturnsFalseIfDirDoesNotExists()
        {
            PolicyResult result = new DirectoryStructurePolicy().Check(new DirectoryInfo("IDoNotExist."));
            Assert.IsFalse(result.Success);
            Assert.That(result.Messages.Count, Is.EqualTo(1));
        }

        [Test]
        public void DirectoryStructurePolicyCheckMustSucceedWithValidStructure()
        {
            // Setup
            DirectoryInfo databaseScriptsRoot =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("DirectoryStructureMustBeCorrectTest");

            DirectoryStructurePolicy directoryStructurePolicy = new DirectoryStructurePolicy();
            PolicyResult result = directoryStructurePolicy.Check(databaseScriptsRoot);
            Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Messages.ToArray()));
            Assert.IsEmpty(result.Messages);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteAllScriptsForDatabaseFromVersionArgumentException()
        {
            new DatabaseReleaseTool(
                Path.GetTempPath(), 
                null, 
                null, 
                new ConsoleOutputLogger(), 
                _connectionStringFactory, 
                CreateMockDatabaseConnectionFactory()).ExecuteAllScriptsForDatabase(string.Empty, "1.0", null, null);
        }

        [Test]
        public void ExecuteAllScriptsForDatabaseFromVersionInvalidVersion()
        {
            ExcecutionResult result =
                new DatabaseReleaseTool(
                    Path.GetTempPath(), 
                    null, 
                    null, 
                    new ConsoleOutputLogger(), 
                    _connectionStringFactory, 
                    CreateMockDatabaseConnectionFactory()).ExecuteAllScriptsForDatabase("invalidversion", "1.0", null, null);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void ExecuteAllScriptsForDatabaseSmokeTest()
        {
            // Setup
            string pathName = "ExecuteAllScriptsForDatabaseSmokeTest";
            DirectoryInfo directoryInfo = DirectoryStructureHelper.CreateValidDatabaseDirStructure(pathName);
            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "1.0_TestScript.sql"));

            IFileStructurePolicy fileStucturePolicyComposite = new FileStructurePolicyComposite();

            IDatabaseConnectionFactory connectionFactory = CreateMockDatabaseConnectionFactory();

            IOutputLogger outputLogger = new ConsoleOutputLogger();
            DatabaseReleaseTool dbreleaseTool = new DatabaseReleaseTool(
                directoryInfo.FullName, 
                null, 
                fileStucturePolicyComposite, 
                outputLogger, 
                _connectionStringFactory, 
                connectionFactory);

            string fromVersion = "1.0.0.0";
            string toVersion = "2.0";

            DatabaseConnectionParameters parameters = new DatabaseConnectionParameters { DatabaseType = DatabaseType.MsSql };
            parameters.CreationType = ConnectionStringCreationType.FromArguments;
            parameters.Arguments.Hostname = ".";
            parameters.Arguments.Username = "testusername";
            parameters.Arguments.Password = "testpassword";
            parameters.Arguments.Database = "testdatabase";

            // Act
            ExcecutionResult result = dbreleaseTool.ExecuteAllScriptsForDatabase(fromVersion, toVersion, parameters, null);

            Assert.IsTrue(result.Success, string.Join("|", result.Errors.ToArray()));
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteAllScriptsForDatabaseToVersionArgumentException()
        {
            new DatabaseReleaseTool(
                Path.GetTempPath(), 
                null, 
                null, 
                new ConsoleOutputLogger(), 
                _connectionStringFactory, 
                CreateMockDatabaseConnectionFactory()).ExecuteAllScriptsForDatabase("1.0", string.Empty, null, null);
        }

        [Test]
        public void ExecuteAllScriptsForDatabaseToVersionInvalidVersion()
        {
            ExcecutionResult result =
                new DatabaseReleaseTool(
                    Path.GetTempPath(), 
                    null, 
                    null, 
                    new ConsoleOutputLogger(), 
                    _connectionStringFactory, 
                    CreateMockDatabaseConnectionFactory()).ExecuteAllScriptsForDatabase("1.0", "invalidversion", null, null);

            Assert.IsFalse(result.Success);
        }

        [Test]
        [TestCase(@"TestFiles\Utf8Encoding.txt", true)]
        [TestCase(@"TestFiles\DefaultEncoding.txt", true)]
        [TestCase(@"TestFiles\Utf8Encoding.txt", false)]
        [TestCase(@"TestFiles\DefaultEncoding.txt", false)]
        public void InputFilesShouldGiveDifferentResultWhenReadWithDifferentEncoding(string inputFile, bool useUtf8)
        {
            // Arrange
            Encoding encodingForReading = useUtf8 ? Encoding.UTF8 : Encoding.Default;
            Encoding encodingForTest = Equals(encodingForReading, Encoding.UTF8) ? Encoding.Default : Encoding.UTF8;

            string readContent = File.ReadAllText(inputFile, encodingForReading);
            string testContent = File.ReadAllText(inputFile, encodingForTest);

            // Assert
            Assert.That(readContent, Is.Not.EqualTo(testContent));
        }

        [Test]
        public void ShouldFailWhenMoreThanOneCreateDatabaseScriptExists()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldFailWhenMoreThanOneCreateDatabaseScriptExists");
            IFileStructurePolicy policy = new MustContainCreateDatabaseSqlFilePolicy();

            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "CreateDatabase.sql"));
            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[2].FullName, "CreateDatabase.sql"));

            PolicyResult result = policy.Check(directoryInfo);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Messages, Has.Count.EqualTo(1));
        }

        [Test]
        public void ShouldSucceedWhenNoCreateDatabaseScriptExists()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldSucceedWhenNoCreateDatabaseScriptExists");
            IFileStructurePolicy policy = new MustContainCreateDatabaseSqlFilePolicy();

            PolicyResult result = policy.Check(directoryInfo);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldSucceedWhenOnlyIgnoredDirectoriesHasCreateDatabaseScript()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldFailWhenOnlyIgnoredDirectoriesHasCreateDatabaseScript");
            IFileStructurePolicy policy = new MustContainCreateDatabaseSqlFilePolicy();

            // Create sql files ONLY in the ignoredirlist
            foreach (string ignoreDir in Resources.DirectoryIgnoreListStructureTemplateList())
            {
                string fullPath = Path.Combine(directoryInfo.FullName, ignoreDir);
                DirectoryStructureHelper.CreateEmptyFile(Path.Combine(fullPath, "CreateDatabase.sql"));
            }

            // Act
            PolicyResult result = policy.Check(directoryInfo);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldSucceedWhenOnlyOneCreateDatabaseScriptExists()
        {
            // Setup
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("ShouldSucceedWhenOnlyOneCreateDatabaseScriptExists");
            IFileStructurePolicy policy = new MustContainCreateDatabaseSqlFilePolicy();

            DirectoryStructureHelper.CreateEmptyFile(Path.Combine(directoryInfo.GetDirectories()[1].FullName, "CreateDatabase.sql"));

            PolicyResult result = policy.Check(directoryInfo);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void TransactionalSqlFileExecutorShouldCallOutputLoggerWriteLineMethod()
        {
            // Arrange
            IDbConnection conn = MockRepository.GenerateStub<IDbConnection>();
            IOutputLogger outputLogger = MockRepository.GenerateMock<IOutputLogger>();
            outputLogger.Expect(method => method.WriteInfoLine("Dummy", 1)).Repeat.Never();
            TransactionalSqlFileExecutor executor = new TransactionalSqlFileExecutor(conn, outputLogger);
            List<string> fileList = new List<string>();

            // Act
            executor.ExecuteTransactional(fileList, null);

            // Assert
            outputLogger.VerifyAllExpectations();
        }

        [Test]
        [TestCase(@"TestFiles\Utf8Encoding.txt", true)]
        [TestCase(@"TestFiles\DefaultEncoding.txt", true)]
        [TestCase(@"TestFiles\Utf8Encoding.txt", false)]
        [TestCase(@"TestFiles\DefaultEncoding.txt", false)]
        public void TransactionalSqlFileExecutorShouldReadFilesWithGivenEncoding(string inputFile, bool useUtf8)
        {
            // Arrange
            IDbConnection conn = MockRepository.GenerateStub<IDbConnection>();
            IOutputLogger outputLogger = MockRepository.GenerateMock<IOutputLogger>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            conn.Expect(method => method.BeginTransaction()).Return(transaction);
            conn.Expect(method => method.CreateCommand()).Return(command);

            List<string> fileList = new List<string> { inputFile };
            Encoding encoding = useUtf8 ? Encoding.UTF8 : Encoding.Default;

            TransactionalSqlFileExecutor executor = new TransactionalSqlFileExecutor(conn, outputLogger);

            // Act
            executor.ExecuteTransactional(fileList, encoding);

            // Assert
            Assert.That(command.CommandText, Is.EqualTo(File.ReadAllText(inputFile, encoding)));
        }

        #endregion

        #region Methods

        private static IDatabaseConnectionFactory CreateMockDatabaseConnectionFactory()
        {
            IDatabaseConnectionFactory connectionFactory = MockRepository.GenerateStub<IDatabaseConnectionFactory>();
            IDbConnection connection = MockRepository.GenerateStub<IDbConnection>();
            IDbTransaction transaction = MockRepository.GenerateStub<IDbTransaction>();
            IDbCommand command = MockRepository.GenerateStub<IDbCommand>();

            connectionFactory.Expect(method => method.CreateDatabaseConnection(string.Empty, DatabaseType.MsSql))
                .IgnoreArguments()
                .Return(connection);
            connection.Expect(method => method.BeginTransaction()).Return(transaction);
            connection.Expect(method => method.CreateCommand()).Return(command);
            return connectionFactory;
        }

        #endregion
    }
}