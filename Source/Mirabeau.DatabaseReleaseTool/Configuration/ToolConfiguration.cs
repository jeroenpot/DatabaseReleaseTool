namespace Mirabeau.DatabaseReleaseTool.Configuration
{
    internal static class ToolConfiguration
    {
        #region Static Fields

        public static readonly string SqlFileExtension = "sql";

        public static readonly string CreateDatabaseSqlFileSearchPattern = string.Format("CreateDatabase.{0}", SqlFileExtension);        

        public static readonly string SqlFileSearchPattern = string.Format("*.{0}", SqlFileExtension);

        public static readonly int DefaultCommandTimeout = 3600;

        #endregion
    }
}