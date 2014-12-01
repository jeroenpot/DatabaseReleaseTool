using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;

namespace Mirabeau.DatabaseReleaseTool.Files
{
    public static class FileListFilter
    {
        #region Public Methods and Operators

        public static IEnumerable<FileInfo> FilterFileListWithIgnoreDirectoryFilter(FileInfo[] allFiles)
        {
            List<string> directoryIgnoreList = Resources.DirectoryIgnoreListStructureTemplateList();

            // remove the ignore directories
            return from fileInfo in allFiles where directoryIgnoreList.Contains(fileInfo.Directory.Name) == false select fileInfo;
        }

        #endregion
    }
}