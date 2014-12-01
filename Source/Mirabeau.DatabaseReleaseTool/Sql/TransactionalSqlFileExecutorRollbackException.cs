using System;

namespace Mirabeau.DatabaseReleaseTool.Sql
{    
    [Serializable]
    public class TransactionalSqlFileExecutorRollbackException : TransactionalSqlFileExecutorException
    {
        #region Constructors and Destructors

        public TransactionalSqlFileExecutorRollbackException(string message, Exception mainException, Exception rollbackException)
            : base(message, rollbackException)
        {
            MainException = mainException;
        }

        #endregion

        #region Public Properties

        public Exception MainException { get; private set; }

        #endregion
    }
}