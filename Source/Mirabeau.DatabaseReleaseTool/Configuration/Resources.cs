using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mirabeau.DatabaseReleaseTool.Configuration
{
    /// <summary>
    /// Extensions on resources.
    /// </summary>
    public partial class Resources
    {
        #region Public Methods and Operators

        public static List<string> DirectoryIgnoreListStructureTemplateList()
        {
            return ReadResourceAsLines(DirectoryIgnoreListStructureTemplate).ToList();
        }

        public static List<string> DirectoryStructureTemplateList()
        {
            return ReadResourceAsLines(DirectoryStructureTemplate).ToList();
        }

        #endregion

        #region Methods

        private static IEnumerable<string> ReadResourceAsLines(string resource)
        {
            using (StringReader reader = new StringReader(resource))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        yield return line;
                    }
                }
            }
        }

        #endregion
    }
}