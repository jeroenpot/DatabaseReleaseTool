using System.Collections.Generic;
using System.IO;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class FileStructurePolicyComposite : IFileStructurePolicy
    {
        #region Fields

        private readonly List<IFileStructurePolicy> _policies = new List<IFileStructurePolicy>();

        #endregion

        #region Constructors and Destructors

        public FileStructurePolicyComposite()
        {
            _policies.Add(new DirectoryStructurePolicy());
            _policies.Add(new MustContainSqlFilePolicy());
            _policies.Add(new FilenamePrefixPolicy());
        }

        #endregion

        #region Public Methods and Operators

        public PolicyResult Check(DirectoryInfo databaseScriptsRoot)
        {
            foreach (IFileStructurePolicy policy in _policies)
            {
                PolicyResult result = policy.Check(databaseScriptsRoot);
                if (!result.Success)
                {
                    return result;
                }
            }

            return new PolicyResult { Success = true };
        }

        #endregion
    }
}