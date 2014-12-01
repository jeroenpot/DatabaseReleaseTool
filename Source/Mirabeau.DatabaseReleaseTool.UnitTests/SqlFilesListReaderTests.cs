using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mirabeau.DatabaseReleaseTool.Files;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class SqlFilesListReaderTests
    {
        #region Public Methods and Operators

        [Test]
        public void SqlReaderShouldFilterFilesBasedOnPassenFromAndToVersion()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("SqlReaderShouldFilterFilesBasedOnPassenFromAndToVersion");
            DirectoryInfo[] subdirs = directoryInfo.GetDirectories();
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.1.0.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.11.0.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.2.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[1], "1.8.0.0_StoredProc.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[1], "1.7.0_StoredProc.sql");

            SqlFilesListReader reader = new SqlFilesListReader(directoryInfo);

            string fromVersion = "1.0";
            string toVersion = "1.7";

            // Act
            Dictionary<string, FileInfo> sqlFilesToExecuteInProperOrder = reader.GetSpecificVersionedFilesToExecute(fromVersion, toVersion);

            // Assert
            Assert.AreEqual(3, sqlFilesToExecuteInProperOrder.Count);
        }

        [Test]
        public void SqlReaderShouldOrderSqlFilesInCorrectSequenceTest()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("SqlReaderShouldOrderSqlFilesInCorrectSequenceTest");
            DirectoryInfo[] subdirs = directoryInfo.GetDirectories();
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.1.0.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.2.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[1], "1.7.0_StoredProc.sql");

            SqlFilesListReader reader = new SqlFilesListReader(directoryInfo);

            string fromVersion = "1.0";
            string toVersion = "1.7";

            // Act
            Dictionary<string, FileInfo> sqlFilesToExecuteInProperOrder = reader.GetSpecificVersionedFilesToExecute(fromVersion, toVersion);

            // Assert
            Assert.AreEqual("1.1.0.0_DataSetup.sql", Path.GetFileName(sqlFilesToExecuteInProperOrder.ElementAt(0).Key));
            Assert.AreEqual("1.2.0_DataSetup.sql", Path.GetFileName(sqlFilesToExecuteInProperOrder.ElementAt(1).Key));
            Assert.AreEqual("1.7.0_StoredProc.sql", Path.GetFileName(sqlFilesToExecuteInProperOrder.ElementAt(2).Key));
        }

        [Test]
        public void SqlReaderShouldReturnAllFilesWhenNullVersionsArePassed()
        {
            // Arrange
            DirectoryInfo directoryInfo =
                DirectoryStructureHelper.CreateValidDatabaseDirStructure("SqlReaderShouldReturnAllFilesWhenNullVersionsArePassed");
            DirectoryInfo[] subdirs = directoryInfo.GetDirectories();
            DirectoryStructureHelper.CreateEmptyFile(subdirs[0], "1.3.0.0_DataSetup.sql");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[1], "1.1.0.0_CreateDatabase.SQL");
            DirectoryStructureHelper.CreateEmptyFile(subdirs[2], "1.2.0_CreateTables.sql");

            SqlFilesListReader reader = new SqlFilesListReader(directoryInfo);

            // Act
            Dictionary<string, FileInfo> sqlFilesToExecuteInProperOrder = reader.GetSpecificVersionedFilesToExecute(null, null as Version);

            // Assert
            Assert.AreEqual(3, sqlFilesToExecuteInProperOrder.Count);
        }

        #endregion
    }
}