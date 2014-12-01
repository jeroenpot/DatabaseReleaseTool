namespace Mirabeau.DatabaseReleaseTool.Logging
{
    public interface IOutputLogger
    {
        #region Public Methods and Operators

        void WriteDebugMessage(string message);

        void WriteErrorMessage(string message);

        void WriteInfoLine(string message, int indentDepth);

        void WriteInfoMessage(string message);

        void WriteSuccessMessage(string message);

        #endregion
    }
}