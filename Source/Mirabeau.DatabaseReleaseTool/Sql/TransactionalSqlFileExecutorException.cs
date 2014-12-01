using System;
using System.Runtime.Serialization;

namespace Mirabeau.DatabaseReleaseTool.Sql
{
    [Serializable]
    public class TransactionalSqlFileExecutorException : Exception
    {
        // For guidelines regarding the creation of new exception types, see
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        #region Constructors and Destructors

        public TransactionalSqlFileExecutorException()
        {
        }

        public TransactionalSqlFileExecutorException(string message)
            : base(message)
        {
        }

        public TransactionalSqlFileExecutorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TransactionalSqlFileExecutorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}