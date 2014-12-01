using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mirabeau.DatabaseReleaseTool.Logging
{
    public class ConsoleOutputLogger : IOutputLogger
    {
        #region Public Methods and Operators

        public void WriteDebugMessage(string message)
        {
            WriteMessage(message, ConsoleColor.DarkGray);
        }

        public void WriteErrorMessage(string message)
        {
            WriteMessage(message, ConsoleColor.Red);
        }

        public void WriteInfoLine(string message, int indentDepth)
        {
            if (indentDepth < 0)
            {
                indentDepth = 0;
            }

            if (!string.IsNullOrEmpty(message))
            {
                WriteInfoMessage(string.Format(CultureInfo.InstalledUICulture, "{0}{1}", Tabs(indentDepth), message));
            }
        }

        public void WriteInfoMessage(string message)
        {
            WriteMessage(message, ConsoleColor.Gray);
        }

        public void WriteSuccessMessage(string message)
        {
            WriteMessage(message, ConsoleColor.Green);
        }

        #endregion

        #region Methods

        private static string Tabs(int numTabs)
        {
            IEnumerable<string> tabs = Enumerable.Repeat("\t", numTabs);
            return (numTabs > 0) ? tabs.Aggregate((sum, next) => sum + next) : string.Empty;
        }

        private static void WriteMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        #endregion
    }
}