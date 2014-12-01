using System.IO;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public abstract class FileStructurePolicyBase : IFileStructurePolicy
    {
        #region Public Methods and Operators

        public PolicyResult Check(DirectoryInfo databaseScriptsRoot)
        {
            if (!databaseScriptsRoot.Exists)
            {
                PolicyResult result = new PolicyResult();
                result.Messages.Add(string.Format("Policy failed. Directory: {0} does not exists.", databaseScriptsRoot.FullName));
                return result;
            }

            return CheckPolicy(databaseScriptsRoot);
        }

        #endregion

        #region Methods

        protected abstract PolicyResult CheckPolicy(DirectoryInfo databaseScriptsRoot);

        #endregion
    }
}