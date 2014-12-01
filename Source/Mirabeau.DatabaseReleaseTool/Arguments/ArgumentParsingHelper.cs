using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public static class ArgumentParsingHelper
    {
        #region Public Methods and Operators

        public static string GetArgumentValueFromArguments(IEnumerable<string> args, string argumentOption)
        {
            string result = GetArgumentForOption(args, argumentOption);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            return result.Substring(argumentOption.Length);
        }

        #endregion

        #region Methods

        private static string GetArgumentForOption(IEnumerable<string> args, string argumentOption)
        {
            IEnumerable<string> result = from argument in args
                where !string.IsNullOrEmpty(argument) && argument.StartsWith(argumentOption, StringComparison.OrdinalIgnoreCase)
                select argument;

            return result.FirstOrDefault();
        }

        #endregion
    }
}