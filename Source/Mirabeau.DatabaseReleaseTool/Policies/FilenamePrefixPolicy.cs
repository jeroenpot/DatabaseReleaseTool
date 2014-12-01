using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Files;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class FilenamePrefixPolicy : FileStructurePolicyBase
    {
        #region Methods

        protected override PolicyResult CheckPolicy(DirectoryInfo databaseScriptsRoot)
        {
            PolicyResult result = new PolicyResult();

            IEnumerable<FileInfo> allFiles =
                FileListFilter.FilterFileListWithIgnoreDirectoryFilter(
                    databaseScriptsRoot.GetFiles(ToolConfiguration.SqlFileSearchPattern, SearchOption.AllDirectories));

            IEnumerable<FileInfo> allFilesWithoutCreateDatabase =
                allFiles.Where(x => !ToolConfiguration.CreateDatabaseSqlFileSearchPattern.Equals(x.Name));

            IEnumerable<FileInfo> invalidFilenames = (from fileInfo in allFilesWithoutCreateDatabase
                where VersionNumberHelper.FilenameHasValidVersionPrefix(fileInfo.Name) == false
                select fileInfo).ToList();

            if (!invalidFilenames.Any())
            {
                result.Success = true;
            }
            else
            {
                result.Messages.Add(
                    string.Format(
                        "FilenamePrefixPolicy failed. The following files do not have a valid prefix: {0}.", 
                        string.Join(",", invalidFilenames.Select(fileinfo => fileinfo.FullName).ToArray())));
            }

            return result;
        }

        #endregion
    }
}