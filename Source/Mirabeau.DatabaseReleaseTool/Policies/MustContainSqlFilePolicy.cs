using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Files;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class MustContainSqlFilePolicy : FileStructurePolicyBase
    {
        #region Methods

        protected override PolicyResult CheckPolicy(DirectoryInfo databaseScriptsRoot)
        {
            PolicyResult result = new PolicyResult();

            FileInfo[] allSqlFiles =
                FileListFilter.FilterFileListWithIgnoreDirectoryFilter(
                    databaseScriptsRoot.GetFiles(ToolConfiguration.SqlFileSearchPattern, SearchOption.AllDirectories)).ToArray();

            if (allSqlFiles.Length == 0)
            {
                result.Messages.Add(
                    string.Format(
                        "MustContainSqlFilePolicy failed. There were no script files with the extension: {0} found.", 
                        ToolConfiguration.SqlFileExtension));
                return result;
            }

            result.Success = true;
            return result;
        }

        #endregion
    }
}