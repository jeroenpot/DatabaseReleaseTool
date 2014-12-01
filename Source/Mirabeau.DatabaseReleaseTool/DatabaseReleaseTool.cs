using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Connection;
using Mirabeau.DatabaseReleaseTool.Files;
using Mirabeau.DatabaseReleaseTool.Logging;
using Mirabeau.DatabaseReleaseTool.Policies;
using Mirabeau.DatabaseReleaseTool.Sql;

namespace Mirabeau.DatabaseReleaseTool
{
    public class DatabaseReleaseTool
    {
        #region Fields

        private readonly IConnectionStringFactory _connectionStringFactory;

        private readonly IFileStructurePolicy _createDatabasePolicy;

        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        private readonly DirectoryInfo _databaseScriptsDirInfo;

        private readonly IFileStructurePolicy _fileStructurePolicy;

        private readonly IOutputLogger _outputLogger;

        #endregion

        #region Constructors and Destructors

        public DatabaseReleaseTool(
            string databaseScriptPath, 
            IFileStructurePolicy createDatabasePolicy, 
            IFileStructurePolicy fileStructurePolicy, 
            IOutputLogger outputLogger, 
            IConnectionStringFactory connectionStringFactory, 
            IDatabaseConnectionFactory databaseConnectionFactory)
        {
            if (outputLogger == null)
            {
                throw new ArgumentNullException("outputLogger");
            }

            if (string.IsNullOrEmpty(databaseScriptPath))
            {
                throw new ArgumentException(@"Cannot be null or empty.", "databaseScriptPath");
            }

            if (!Directory.Exists(databaseScriptPath))
            {
                throw new DirectoryNotFoundException(string.Format("Path: {0} does not exist.", databaseScriptPath));
            }

            _createDatabasePolicy = createDatabasePolicy;
            _fileStructurePolicy = fileStructurePolicy;
            _outputLogger = outputLogger;
            _connectionStringFactory = connectionStringFactory;
            _databaseConnectionFactory = databaseConnectionFactory;
            _databaseScriptsDirInfo = new DirectoryInfo(databaseScriptPath);
        }

        #endregion

        #region Public Methods and Operators

        public ExcecutionResult Execute(
            string fromVersion, 
            string toVersion, 
            DatabaseConnectionParameters connectionParameters, 
            Encoding encodingForReadingSqlScripts)
        {
            var excecutionResult = new ExcecutionResult { Success = true };

            if (connectionParameters.BeforeExecuteScriptsAction == BeforeExecuteScriptsAction.CreateDatabase)
            {
                excecutionResult = ExecuteCreateDatabase(connectionParameters, encodingForReadingSqlScripts);
            }

            if (excecutionResult.Success)
            {
                excecutionResult = ExecuteAllScriptsForDatabase(fromVersion, toVersion, connectionParameters, encodingForReadingSqlScripts);
            }

            return excecutionResult;
        }

        public virtual ExcecutionResult ExecuteAllScriptsForDatabase(
            string fromVersion, 
            string toVersion, 
            DatabaseConnectionParameters connectionParameters, 
            Encoding encodingForReadingSqlScripts)
        {
            if (string.IsNullOrEmpty(fromVersion))
            {
                throw new ArgumentException(@"Cannot be null or empty.", "fromVersion");
            }

            if (string.IsNullOrEmpty(toVersion))
            {
                throw new ArgumentException(@"Cannot be null or empty.", "toVersion");
            }

            if (encodingForReadingSqlScripts == null)
            {
                encodingForReadingSqlScripts = Encoding.Default;
            }

            Version fromVersionObject;
            Version toVersionObject;

            try
            {
                fromVersionObject = VersionNumberHelper.RemoveNegativeNumbersFromVersionObject(new Version(fromVersion));
            }
            catch (Exception ex)
            {
                var result = new ExcecutionResult { Success = false };
                result.Errors.Add(
                    string.Format("Invalid fromVersion. Could not parse fromVersion to a usable version object. {0}", ex.Message));
                return result;
            }

            try
            {
                toVersionObject = VersionNumberHelper.RemoveNegativeNumbersFromVersionObject(new Version(toVersion));
            }
            catch (Exception ex)
            {
                var result = new ExcecutionResult { Success = false };
                result.Errors.Add(string.Format("Invalid toVersion. Could not parse toVersion to a usable version object. {0}", ex.Message));
                return result;
            }

            // Execute optional FileStructurePolicy
            if (_fileStructurePolicy != null)
            {
                PolicyResult policyResult = _fileStructurePolicy.Check(_databaseScriptsDirInfo);

                if (!policyResult.Success)
                {
                    // Policy failed Return errors
                    ExcecutionResult result = new ExcecutionResult { Success = false };
                    result.Errors.AddRange(policyResult.Messages);
                    return result;
                }
            }

            SqlFilesListReader filesReader = new SqlFilesListReader(_databaseScriptsDirInfo);
            Dictionary<string, FileInfo> filesToExecute = filesReader.GetSpecificVersionedFilesToExecute(fromVersionObject, toVersionObject);

            return ExecuteSqlScripts(connectionParameters, filesToExecute, encodingForReadingSqlScripts);
        }

        public virtual ExcecutionResult ExecuteCreateDatabase(
            DatabaseConnectionParameters connectionParameters, 
            Encoding encodingForReadingSqlScripts)
        {
            // Execute optional FileStructurePolicy
            if (_createDatabasePolicy != null)
            {
                PolicyResult policyResult = _createDatabasePolicy.Check(_databaseScriptsDirInfo);

                if (!policyResult.Success)
                {
                    // Policy failed Return errors
                    ExcecutionResult result = new ExcecutionResult { Success = false };
                    result.Errors.AddRange(policyResult.Messages);
                    return result;
                }
            }

            SqlFilesListReader filesReader = new SqlFilesListReader(_databaseScriptsDirInfo);

            Dictionary<string, FileInfo> filesToExecute = filesReader.GetCreateDatabaseFileToExecute();

            return ExecuteNonTransactionalCreateDatabaseSqlScripts(connectionParameters, filesToExecute, encodingForReadingSqlScripts);
        }

        #endregion

        #region Methods

        private IDbConnection CreateDbConnection(DatabaseConnectionParameters connectionParameters)
        {
            string connectionString = _connectionStringFactory.Create(connectionParameters);
            return _databaseConnectionFactory.CreateDatabaseConnection(connectionString, connectionParameters.DatabaseType);
        }

        private ExcecutionResult ExecuteNonTransactionalCreateDatabaseSqlScripts(
            DatabaseConnectionParameters connectionParameters, 
            Dictionary<string, FileInfo> filesToExecute, 
            Encoding encodingForReadingSqlScripts)
        {
            IDbConnection connection = CreateDbConnection(connectionParameters);
            TransactionalSqlFileExecutor sqlFileExecutor = new TransactionalSqlFileExecutor(connection, _outputLogger);

            try
            {
                sqlFileExecutor.ExecuteNonTransactional(filesToExecute.Select(item => item.Key).ToList(), encodingForReadingSqlScripts);
            }
            catch (TransactionalSqlFileExecutorException exception)
            {
                var result = new ExcecutionResult { Success = false };
                result.Errors.Add(exception.Message);
                result.Errors.Add(exception.InnerException.Message);
                return result;
            }

            return new ExcecutionResult { Success = true };
        }

        private ExcecutionResult ExecuteSqlScripts(
            DatabaseConnectionParameters connectionParameters, 
            Dictionary<string, FileInfo> filesToExecute, 
            Encoding encodingForReadingSqlScripts)
        {
            connectionParameters.BeforeExecuteScriptsAction = BeforeExecuteScriptsAction.None;
            IDbConnection connection = CreateDbConnection(connectionParameters);
            TransactionalSqlFileExecutor sqlFileExecutor = new TransactionalSqlFileExecutor(connection, _outputLogger);

            try
            {
                sqlFileExecutor.ExecuteTransactional(filesToExecute.Select(item => item.Key).ToList(), encodingForReadingSqlScripts);
            }
            catch (TransactionalSqlFileExecutorRollbackException exception)
            {
                var result = new ExcecutionResult { Success = false };
                result.Errors.Add(exception.Message);
                result.Errors.Add(exception.MainException.Message);
                result.Errors.Add(exception.InnerException.Message);
                return result;
            }
            catch (TransactionalSqlFileExecutorException exception)
            {
                var result = new ExcecutionResult { Success = false };
                result.Errors.Add(exception.Message);
                result.Errors.Add(exception.InnerException.Message);
                return result;
            }

            return new ExcecutionResult { Success = true };
        }

        #endregion
    }
}