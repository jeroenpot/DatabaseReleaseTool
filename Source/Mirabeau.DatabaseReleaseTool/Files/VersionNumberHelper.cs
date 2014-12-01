using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mirabeau.DatabaseReleaseTool.Files
{
    public class VersionNumberHelper
    {
        #region Public Methods and Operators

        public static bool FilenameHasValidVersionPrefix(string filename)
        {
            // Try to create a version object
            try
            {
                Version version = GetVersionForFilename(filename);

                if (version == null)
                {
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // A major, minor, build, or revision component is less than zero. 
                return false;
            }
            catch (ArgumentException)
            {
                // fewer than 2 and more than 4 components
                return false;
            }
            catch (FormatException)
            {
                // At least one component of version does not parse to an integer. 
                return false;
            }
            catch (OverflowException)
            {
                // At least one component of version represents a number greater than MaxValue. 
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks the filename prefix and returns a <see cref="Version" /> object for the filename.
        /// </summary>
        /// <param name="filename">The filename to get the version prefix from.</param>
        /// <returns>A valid version object or null if it is not a valid version.</returns>
        /// <remarks>
        ///     If the version number prefix does not conform to the <see cref="Version" /> object initialization it will throw an
        ///     exception.
        /// </remarks>
        public static Version GetVersionForFilename(string filename)
        {
            // First remove the file extension
            string workFilename = Path.GetFileNameWithoutExtension(filename);

            // Zoek het laatste getal na een punt
            // Als er geen getal na een punt komt gebruik dan het laatste getal. 1.0.1.350

            // Take the first 4 entries. A version number can contain max 4 numbers. 
            // The take will return max 4 and fewer elements if they are not present.            
            const string VersionDelimiter = ".";

            IEnumerable<string> possibleNumbers =
                workFilename.Split(new[] { VersionDelimiter }, StringSplitOptions.RemoveEmptyEntries).Take(4);

            if (!possibleNumbers.Any())
            {
                // No version .'s found so no valid version number prefix.
                return null;
            }

            // Check each character in the last part. The first occurence of a non integer value char is the index of the last character wich contains the number.
            string versionString = GetVersionString(VersionDelimiter, possibleNumbers);

            // Try to create a version object          
            Version resultVersion = new Version(versionString);

            Version properVersion = RemoveNegativeNumbersFromVersionObject(resultVersion);

            return properVersion;
        }

        /// <summary>
        ///     Removes the -1's in the revision and build numbers. Which are automatically set if those are not gives as input to the
        ///     version.
        /// </summary>
        /// <param name="resultVersion">The version from which the negative numbers should be removed.</param>
        /// <returns>The version with no negative numbers. Negative numbers will be set to 0.</returns>
        public static Version RemoveNegativeNumbersFromVersionObject(Version resultVersion)
        {
            int major = resultVersion.Major;
            int minor = resultVersion.Minor;

            int revision = resultVersion.Minor == -1 ? 0 : resultVersion.Minor;
            int build = resultVersion.Build == -1 ? 0 : resultVersion.Build;

            // Make sure there are no -1 in the version. Gives odd behavior in comparing.
            return new Version(major, minor, build, revision);
        }

        #endregion

        #region Methods

        private static int GetIndexOfLastCharacterWichIsNotANumber(string lastVersionnumberpart)
        {
            int indexOfLastCharacterWichIsNotANumber = -1;

            for (int i = 1; i <= lastVersionnumberpart.Length; i++)
            {
                string possibleNumber = lastVersionnumberpart.Substring(0, i);
                int result;

                if (!int.TryParse(possibleNumber, out result))
                {
                    indexOfLastCharacterWichIsNotANumber = i;
                    return indexOfLastCharacterWichIsNotANumber;
                }
            }

            return indexOfLastCharacterWichIsNotANumber;
        }

        private static string GetVersionString(string versionDelimiter, IEnumerable<string> possibleNumbers)
        {
            string versionString;
            string lastVersionnumberpart = possibleNumbers.Last();

            int indexOfLastCharacterWichIsNotANumber = GetIndexOfLastCharacterWichIsNotANumber(lastVersionnumberpart);

            if (indexOfLastCharacterWichIsNotANumber == -1)
            {
                // the whole last part is a number, do nothing.
                versionString = string.Join(versionDelimiter, possibleNumbers.ToArray());
            }
            else
            {
                string lastVersionNumberPart = lastVersionnumberpart.Substring(0, indexOfLastCharacterWichIsNotANumber - 1);
                List<string> possibleNumbersList = possibleNumbers.ToList();

                // Remove last item
                possibleNumbersList.RemoveAt(possibleNumbersList.Count - 1);

                // Insert proper lastVersionNumberPart
                possibleNumbersList.Add(lastVersionNumberPart);
                versionString = string.Join(versionDelimiter, possibleNumbersList.ToArray());
            }

            return versionString;
        }

        #endregion
    }
}