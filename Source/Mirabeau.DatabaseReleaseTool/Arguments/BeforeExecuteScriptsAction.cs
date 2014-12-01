namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public enum BeforeExecuteScriptsAction
    {
        /// <summary>
        /// No action has to be taken, default.
        /// </summary>
        None, 

        /// <summary>
        /// Create database before all scripts are executed.
        /// </summary>
        CreateDatabase
    }
}