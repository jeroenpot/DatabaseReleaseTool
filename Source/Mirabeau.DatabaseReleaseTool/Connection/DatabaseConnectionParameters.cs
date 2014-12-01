using Mirabeau.DatabaseReleaseTool.Arguments;

namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public class DatabaseConnectionParameters
    {
        #region Constructors and Destructors

        public DatabaseConnectionParameters()
        {
            Arguments = new DatabaseConnectionArguments();
        }

        #endregion

        #region Public Properties

        public DatabaseConnectionArguments Arguments { get; set; }

        public BeforeExecuteScriptsAction BeforeExecuteScriptsAction { get; set; }

        public string ConfigurationFileName { get; set; }

        public ConnectionStringCreationType CreationType { get; set; }

        public DatabaseType DatabaseType { get; set; }

        #endregion
    }
}