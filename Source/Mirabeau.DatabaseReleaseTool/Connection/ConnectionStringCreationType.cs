namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public enum ConnectionStringCreationType
    {
        /// <summary>
        /// ConnectionString creation from config file.
        /// </summary>
        FromConfigurationFile,

        /// <summary>
        /// ConnectionString creation from config CommandLine arguments.
        /// </summary>
        FromArguments
    }
}