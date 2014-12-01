using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;

namespace Mirabeau.DatabaseReleaseTool.Files
{
    /// <summary>
    ///     This class reads sql files from a directory and all it's  subdirectories.
    /// </summary>
    public class SqlFilesListReader
    {
        #region Fields

        private readonly DirectoryInfo databaseScriptsRoot;

        #endregion

        #region Constructors and Destructors

        public SqlFilesListReader(DirectoryInfo databaseScriptsRoot)
        {
            this.databaseScriptsRoot = databaseScriptsRoot;
        }

        #endregion

        #region Public Methods and Operators

        public Dictionary<string, FileInfo> GetCreateDatabaseFileToExecute()
        {
            // Get all sql files, unsorted
            IEnumerable<FileInfo> allSqlFiles =
                FileListFilter.FilterFileListWithIgnoreDirectoryFilter(
                    databaseScriptsRoot.GetFiles(ToolConfiguration.CreateDatabaseSqlFileSearchPattern, SearchOption.AllDirectories));
            Dictionary<string, FileInfo> result = allSqlFiles.ToDictionary(key => key.FullName);

            return result;
        }

        public Dictionary<string, FileInfo> GetSpecificVersionedFilesToExecute(string fromVersion, string toVersion)
        {
            Version fromVersionObject = VersionNumberHelper.RemoveNegativeNumbersFromVersionObject(new Version(fromVersion));
            Version toVersionObject = VersionNumberHelper.RemoveNegativeNumbersFromVersionObject(new Version(toVersion));

            return GetSpecificVersionedFilesToExecute(fromVersionObject, toVersionObject);
        }

        public Dictionary<string, FileInfo> GetSpecificVersionedFilesToExecute(Version fromVersionObject, Version toVersionObject)
        {
            Dictionary<string, FileInfo> sqlFilesToExecuteInProperOrder = GetAllFilesToExecute();

            IComparer<string> versionComparer = new VersionComparer();

            IOrderedEnumerable<FileInfo> list = (from item in sqlFilesToExecuteInProperOrder
                let fileVersion = VersionNumberHelper.GetVersionForFilename(item.Key)
                where
                    ((fromVersionObject == null) || (fileVersion >= fromVersionObject))
                    && ((toVersionObject == null) || (fileVersion <= toVersionObject))
                select item.Value).OrderBy(obj => obj.FullName, versionComparer);

            return list.ToDictionary(item => item.FullName);
        }

        #endregion

        #region Methods

        private Dictionary<string, FileInfo> GetAllFilesToExecute()
        {
            // Get all sql files, unsorted
            IEnumerable<FileInfo> allSqlFiles =
                FileListFilter.FilterFileListWithIgnoreDirectoryFilter(
                    databaseScriptsRoot.GetFiles(ToolConfiguration.SqlFileSearchPattern, SearchOption.AllDirectories));

            Dictionary<string, FileInfo> result =
                allSqlFiles.Where(x => !ToolConfiguration.CreateDatabaseSqlFileSearchPattern.Equals(x.Name))
                    .ToDictionary(key => key.FullName);

            return result;
        }

        #endregion
    }
}