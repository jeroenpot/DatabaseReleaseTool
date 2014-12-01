using System.Collections.Generic;

namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public class ArgumentValidationResult
    {
        public ArgumentValidationResult()
        {
            ValidationMessages = new List<string>();
        }

        public ArgumentValidationStatus Status { get; set; }

        public List<string> ValidationMessages { get; private set; }
    }
}