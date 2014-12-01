using System.IO;

using Mirabeau.DatabaseReleaseTool.Configuration;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    internal static class DirectoryStructureHelper
    {
        #region Static Fields

        private static readonly string tempPath = Path.GetTempPath();

        #endregion

        #region Public Methods and Operators

        public static void CreateEmptyFile(DirectoryInfo baseDir, string fileNameWithoutPath)
        {
            CreateEmptyFile(Path.Combine(baseDir.FullName, fileNameWithoutPath));
        }

        public static void CreateEmptyFile(string fileName)
        {
            using (File.Create(fileName))
            {
            }
        }

        public static DirectoryInfo CreateValidDatabaseDirStructure(string databaseScriptsPathName)
        {
            DirectoryInfo testRoot = new DirectoryInfo(tempPath);
            string structurePath = Path.Combine(testRoot.FullName, databaseScriptsPathName);

            // Always clean the dir first.
            if (Directory.Exists(structurePath))
            {
                Directory.Delete(structurePath, true);
            }

            DirectoryInfo structure = testRoot.CreateSubdirectory(databaseScriptsPathName);

            foreach (string line in Resources.DirectoryStructureTemplateList())
            {
                structure.CreateSubdirectory(line);
                DirectoryInfo svnDirInfo = structure.CreateSubdirectory(".svn");
                svnDirInfo.Attributes = FileAttributes.Hidden;
            }

            foreach (string ignoredDir in Resources.DirectoryIgnoreListStructureTemplateList())
            {
                structure.CreateSubdirectory(ignoredDir);
            }

            return structure;
        }

        #endregion
    }
}