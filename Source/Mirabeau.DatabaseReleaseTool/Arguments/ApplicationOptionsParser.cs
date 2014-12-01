using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public class ApplicationOptionsParser : IApplicationOptionsParser
    {
        #region Public Methods and Operators

        public ApplicationOptions Parse(string[] argumentList)
        {
            ApplicationOptions applicationOptions = new ApplicationOptions();

            applicationOptions.DatabaseScriptsPath = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.DatabasePathArgumentOption);
            applicationOptions.Username = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.UsernameArgumentOption);
            applicationOptions.Password = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.PasswordArgumentOption);
            applicationOptions.Servername = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.ServernameArgumentOption);
            applicationOptions.DatabaseName = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.DatabasenameArgumentOption);
            applicationOptions.FromVersion = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.FromDatabaseVersion);
            applicationOptions.ToVersion = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.ToDatabaseVersion);
            applicationOptions.DatabaseType =
                ParseDatabaseType(
                    ArgumentParsingHelper.GetArgumentValueFromArguments(argumentList, CommandLineArgumentOptions.DatabasetypeArgumentOption));
            applicationOptions.ConfigurationFilename = ArgumentParsingHelper.GetArgumentValueFromArguments(
                argumentList, 
                CommandLineArgumentOptions.ConfigFileArgumentOption);
            applicationOptions.InformationSwitch = ParseInformationSwitch(argumentList);
            applicationOptions.BeforeExecuteScriptsAction = ParseBeforeExecuteScriptsAction(argumentList);
            applicationOptions.EncodingForReadingSqlScripts = ParseEncodingForReadingSqlScripts(argumentList);

            return applicationOptions;
        }

        #endregion

        #region Methods

        private static bool IsOptionPresent(IEnumerable<string> argumentList, string option)
        {
            return
                argumentList.Any(argument => !string.IsNullOrEmpty(argument) && argument.Equals(option, StringComparison.OrdinalIgnoreCase));
        }

        private static BeforeExecuteScriptsAction ParseBeforeExecuteScriptsAction(IEnumerable<string> argumentList)
        {
            bool createDatabase = IsOptionPresent(argumentList, CommandLineArgumentOptions.CreateDatabasergumentOption);
            return createDatabase ? BeforeExecuteScriptsAction.CreateDatabase : BeforeExecuteScriptsAction.None;
        }

        private static DatabaseType ParseDatabaseType(string databaseType)
        {
            if (string.IsNullOrEmpty(databaseType))
            {
                return DatabaseType.None;
            }

            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType, true);
        }

        private static Encoding ParseEncodingForReadingSqlScripts(IEnumerable<string> argumentList)
        {
            bool useUtf8Encoding = IsOptionPresent(argumentList, CommandLineArgumentOptions.EncodingUtf8);
            return useUtf8Encoding ? Encoding.UTF8 : Encoding.Default;
        }

        private static InformationSwitch ParseInformationSwitch(IEnumerable<string> argumentList)
        {
            bool showInformation = IsOptionPresent(argumentList, CommandLineArgumentOptions.InfoArgumentOption);
            return showInformation ? InformationSwitch.On : InformationSwitch.None;
        }

        #endregion
    }
}