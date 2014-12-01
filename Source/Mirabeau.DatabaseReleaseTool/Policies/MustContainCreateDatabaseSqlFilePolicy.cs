using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Files;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class MustContainCreateDatabaseSqlFilePolicy : FileStructurePolicyBase
    {
        #region Methods

        protected override PolicyResult CheckPolicy(DirectoryInfo databaseScriptsRoot)
        {
            PolicyResult result = new PolicyResult();

            FileInfo[] allCreateDbSqlFiles =
                FileListFilter.FilterFileListWithIgnoreDirectoryFilter(
                    databaseScriptsRoot.GetFiles(ToolConfiguration.CreateDatabaseSqlFileSearchPattern, SearchOption.AllDirectories))
                    .ToArray();

            if (allCreateDbSqlFiles.Length > 1)
            {
                result.Messages.Add(
                    string.Format(
                        "MustContainCreateDatabaseSqlFilePolicy failed. There are more than one files with the name: {0} found.", 
                        ToolConfiguration.CreateDatabaseSqlFileSearchPattern));
                return result;
            }

            result.Success = true;
            return result;
        }

        #endregion
    }
}