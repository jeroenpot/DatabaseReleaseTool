using System.Data.OracleClient;

using Mirabeau.DatabaseReleaseTool.Files;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class VersionNumberHelperTest
    {
        #region Public Methods and Operators

        [Test]
        public void FilenameHasInvalidVersionPrefix()
        {
            AssertThatFilenameHasInValidPrefix("aaaa.txt");
            AssertThatFilenameHasInValidPrefix("bbbb");
            AssertThatFilenameHasInValidPrefix("A.12.124._001.sql");
            AssertThatFilenameHasInValidPrefix("A.b.c._001.sql");
            AssertThatFilenameHasInValidPrefix("A.b.-1111._001.sql");
        }

        [Test]
        public void FilenameHasValidVersionPrefixSmokeTest()
        {
            AssertThatFilenameHasValidPrefix("1.0.1.436_001.txt");
            AssertThatFilenameHasValidPrefix("1.0.1.436.txt");
            AssertThatFilenameHasValidPrefix("1.0_001.sql");
            AssertThatFilenameHasValidPrefix("2.11.35.872_001.sql");
            AssertThatFilenameHasValidPrefix("1.0_Dinges.sql");
            AssertThatFilenameHasValidPrefix("1.0.536Dinges.sql");
        }

        [Test]
        public void TestConnectionString()
        {
#pragma warning disable 618
            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder();
#pragma warning restore 618
            builder.DataSource = "localhost";
            builder.UserID = "testuserid";
            builder.Password = "testpassword";
            Assert.IsNotEmpty(builder.ConnectionString);
        }

        #endregion

        #region Methods

        private static void AssertThatFilenameHasInValidPrefix(string filenameToTest)
        {
            Assert.IsFalse(
                VersionNumberHelper.FilenameHasValidVersionPrefix(filenameToTest), 
                string.Format("{0} does containt a valid version number.", filenameToTest));
        }

        private static void AssertThatFilenameHasValidPrefix(string filenameToTest)
        {
            Assert.IsTrue(
                VersionNumberHelper.FilenameHasValidVersionPrefix(filenameToTest), 
                string.Format("{0} does not contains a valid version number.", filenameToTest));
        }

        #endregion
    }
}