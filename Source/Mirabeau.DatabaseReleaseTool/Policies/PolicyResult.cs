using System.Collections.Generic;

namespace Mirabeau.DatabaseReleaseTool.Policies
{
    public class PolicyResult
    {
        #region Fields

        private readonly List<string> messages = new List<string>();

        #endregion

        #region Public Properties

        public List<string> Messages
        {
            get
            {
                return messages;
            }
        }

        public bool Success { get; set; }

        #endregion
    }
}