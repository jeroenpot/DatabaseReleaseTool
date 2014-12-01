using System;
using System.Collections.Generic;
using System.IO;

namespace Mirabeau.DatabaseReleaseTool.Files
{
    /// <summary>
    ///     This class is a VersionObject Comparer.
    /// </summary>
    public class VersionComparer : IComparer<string>
    {
        #region Public Methods and Operators

        public int Compare(string x, string y)
        {
            string leftPath = Path.GetDirectoryName(x);
            string rightPath = Path.GetDirectoryName(y);

            int pathcompare = string.CompareOrdinal(leftPath, rightPath);

            if (pathcompare == 0)
            {
                Version leftVersion = VersionNumberHelper.GetVersionForFilename(x);
                Version rightVersion = VersionNumberHelper.GetVersionForFilename(y);

                return leftVersion.CompareTo(rightVersion);
            }

            return pathcompare;
        }

        #endregion
    }
}