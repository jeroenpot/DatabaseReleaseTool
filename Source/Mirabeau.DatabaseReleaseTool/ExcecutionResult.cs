using System.Collections.Generic;

namespace Mirabeau.DatabaseReleaseTool
{
    public class ExcecutionResult
    {
        #region Constructors and Destructors

        public ExcecutionResult()
        {
            Errors = new List<string>();
        }

        #endregion

        #region Public Properties

        public List<string> Errors { get; private set; }

        public bool Success { get; set; }

        #endregion
    }
}