using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Logging;

namespace Mirabeau.DatabaseReleaseTool.Sql
{
    public class TransactionalSqlFileExecutor
    {
        #region Fields

        private readonly IDbConnection _connection;

        private readonly IOutputLogger _outputLogger;

        #endregion

        #region Constructors and Destructors

        public TransactionalSqlFileExecutor(IDbConnection connection, IOutputLogger outputLogger)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (outputLogger == null)
            {
                throw new ArgumentNullException("outputLogger");
            }

            _connection = connection;
            _outputLogger = outputLogger;
        }

        #endregion

        #region Public Methods and Operators

        public void ExecuteNonTransactional(List<string> fileList, Encoding encodingForReadingSqlScripts)
        {
            if (fileList == null)
            {
                throw new ArgumentNullException("fileList");
            }

            if (fileList.Count == 0)
            {
                return;
            }

            using (_connection)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                ExcecuteAllSqlFiles(fileList, encodingForReadingSqlScripts);
            }
        }

        public void ExecuteTransactional(List<string> fileList, Encoding encodingForReadingSqlScripts)
        {
            if (fileList == null)
            {
                throw new ArgumentNullException("fileList");
            }

            if (fileList.Count == 0)
            {
                return;
            }

            using (_connection)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                ExcecuteAllSqlFilesInTransaction(fileList, encodingForReadingSqlScripts);
            }
        }

        #endregion

        #region Methods

        private void ExcecuteAllSqlFiles(IEnumerable<string> fileList, Encoding encodingForReadingSqlScripts)
        {
            foreach (string filename in fileList)
            {
                _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "Start executing file '{0}'", filename), 2);
                try
                {
                    using (IDbCommand command = _connection.CreateCommand())
                    {
                        string commandText = File.ReadAllText(filename, encodingForReadingSqlScripts);
                        command.CommandText = commandText;
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = ToolConfiguration.DefaultCommandTimeout;
                        int affectedRows = command.ExecuteNonQuery();
                        _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "AffectedRows:[{0}]", affectedRows), 3);
                    }
                }
                catch (Exception e)
                {
                    throw new TransactionalSqlFileExecutorException(
                        string.Format(CultureInfo.InstalledUICulture, "An error has occured wile excecuting sql file: {0}.", filename), 
                        e);
                }

                _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "Finished executing file '{0}'", filename), 2);
            }
        }

        private void ExcecuteAllSqlFilesInTransaction(IEnumerable<string> fileList, Encoding encodingForReadingSqlScripts)
        {
            IDbTransaction transaction = _connection.BeginTransaction();

            try
            {
                foreach (string filename in fileList)
                {
                    _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "Start executing file '{0}'", filename), 2);
                    ExecuteOneFileInCommand(filename, transaction, encodingForReadingSqlScripts);
                    _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "Finished executing file '{0}'", filename), 2);
                }

                _outputLogger.WriteInfoLine("Starting to commit transaction", 1);
                transaction.Commit();
                _outputLogger.WriteInfoLine("Finished commiting transaction", 1);
            }
            catch (TransactionalSqlFileExecutorException exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackBackTransaction)
                {
                    TransactionalSqlFileExecutorRollbackException newExc =
                        new TransactionalSqlFileExecutorRollbackException(
                            "Error while rollbacking transaction.", 
                            exception, 
                            rollbackBackTransaction);
                    throw newExc;
                }

                throw;
            }
        }

        private void ExecuteOneFileInCommand(string filename, IDbTransaction transaction, Encoding encodingForReadingSqlScripts)
        {
            try
            {
                using (IDbCommand command = _connection.CreateCommand())
                {
                    string commandText = File.ReadAllText(filename, encodingForReadingSqlScripts);
                    command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = ToolConfiguration.DefaultCommandTimeout;
                    command.Transaction = transaction;
                    int affectedRows = command.ExecuteNonQuery();
                    _outputLogger.WriteInfoLine(string.Format(CultureInfo.InstalledUICulture, "AffectedRows:[{0}]", affectedRows), 3);
                }
            }
            catch (Exception e)
            {
                throw new TransactionalSqlFileExecutorException(
                    string.Format(CultureInfo.InstalledUICulture, "An error has occured wile excecuting sql file: {0}.", filename), 
                    e);
            }
        }

        #endregion
    }
}