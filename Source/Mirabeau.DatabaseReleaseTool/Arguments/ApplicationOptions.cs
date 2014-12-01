using System.Text;

namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public class ApplicationOptions
    {
        #region Public Properties

        public BeforeExecuteScriptsAction BeforeExecuteScriptsAction { get; set; }

        public string ConfigurationFilename { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseScriptsPath { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public Encoding EncodingForReadingSqlScripts { get; set; }

        public string FromVersion { get; set; }

        public InformationSwitch InformationSwitch { get; set; }

        public string Password { get; set; }

        public string Servername { get; set; }

        public string ToVersion { get; set; }

        public string Username { get; set; }

        #endregion
    }
}