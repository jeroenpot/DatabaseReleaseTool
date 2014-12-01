using System;
using System.Collections.Generic;

using Mirabeau.DatabaseReleaseTool.Configuration;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class ResourcesTests
    {
        #region Public Methods and Operators

        [Test]
        public void DirectoryIgnoreListStructureTemplateListShouldReturnTemplateAsAList()
        {
            List<string> directoryTemplateList = Resources.DirectoryIgnoreListStructureTemplateList();

            string[] expectedResult = Resources.DirectoryIgnoreListStructureTemplate.Split(
                new[] { Environment.NewLine, "\r", "\n" }, 
                StringSplitOptions.RemoveEmptyEntries);

            Assert.That(directoryTemplateList, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public void DirectoryStructureTemplateListShouldReturnTemplateAsAList()
        {
            List<string> directoryTemplateList = Resources.DirectoryStructureTemplateList();

            string[] expectedResult = Resources.DirectoryStructureTemplate.Split(
                new[] { Environment.NewLine, "\r", "\n" }, 
                StringSplitOptions.RemoveEmptyEntries);

            Assert.That(directoryTemplateList, Is.EquivalentTo(expectedResult));
        }

        #endregion
    }
}