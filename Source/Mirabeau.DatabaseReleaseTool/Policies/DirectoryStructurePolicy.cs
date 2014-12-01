using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Configuration;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class DirectoryStructurePolicy : FileStructurePolicyBase
    {
        #region Methods

        protected override PolicyResult CheckPolicy(DirectoryInfo databaseScriptsRoot)
        {
            PolicyResult result = new PolicyResult();

            List<string> directoryTemplateList = Resources.DirectoryStructureTemplateList();

            List<string> directoryIgnoreList = Resources.DirectoryIgnoreListStructureTemplateList();

            List<string> actualDirectoryList =
                databaseScriptsRoot.GetDirectories()
                    .Where(dir => ((dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
                    .Select(dirName => dirName.Name)
                    .ToList();

            // remove the ignorelist directories from the actuallist
            directoryIgnoreList.ForEach(ignoreDirectory => actualDirectoryList.Remove(ignoreDirectory));

            // Compare the count            
            if (directoryTemplateList.Count == actualDirectoryList.Count)
            {
                // CheckPolicy if all the directories are present
                IEnumerable<string> directoriesNotPresent = from notListedDir in actualDirectoryList
                    where directoryTemplateList.Contains(notListedDir) == false
                    select notListedDir;

                if (!directoriesNotPresent.Any())
                {
                    result.Success = true;
                }
                else
                {
                    result.Messages.Add(
                        string.Format(
                            "DirectoryStructurePolicy failed. The following directories arent named correctly: {0} .", 
                            string.Join(Environment.NewLine, directoriesNotPresent.ToArray())));
                }
            }

            // Add default failed message if success is false
            if (result.Success == false)
            {
                result.Messages.Add(
                    string.Format(
                        "DirectoryStructurePolicy failed. Target path {0} does not conform to specified directory layout.\n[Layout: \n{1}\n]", 
                        databaseScriptsRoot, 
                        Resources.DirectoryStructureTemplate));
            }

            return result;
        }

        #endregion
    }
}