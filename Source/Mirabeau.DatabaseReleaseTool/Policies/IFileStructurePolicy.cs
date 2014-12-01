using System.IO;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public interface IFileStructurePolicy
    {
        #region Public Methods and Operators

        PolicyResult Check(DirectoryInfo databaseScriptsRoot);

        #endregion
    }
}