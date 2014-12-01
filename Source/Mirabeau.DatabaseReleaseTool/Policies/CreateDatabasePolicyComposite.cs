using System.Collections.Generic;
using System.IO;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class CreateDatabasePolicyComposite : IFileStructurePolicy
    {
        #region Fields

        private readonly List<IFileStructurePolicy> policies = new List<IFileStructurePolicy>();

        #endregion

        #region Constructors and Destructors

        public CreateDatabasePolicyComposite()
        {
            policies.Add(new DirectoryStructurePolicy());
            policies.Add(new MustContainCreateDatabaseSqlFilePolicy());
        }

        #endregion

        #region Public Methods and Operators

        public PolicyResult Check(DirectoryInfo databaseScriptsRoot)
        {
            foreach (IFileStructurePolicy policy in policies)
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