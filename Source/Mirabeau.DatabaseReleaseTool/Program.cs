using System;
using System.Configuration;
using System.Globalization;
using System.IO;

using Mirabeau.DatabaseReleaseTool.Arguments;
using Mirabeau.DatabaseReleaseTool.Configuration;
using Mirabeau.DatabaseReleaseTool.Connection;
using Mirabeau.DatabaseReleaseTool.Logging;
using Mirabeau.DatabaseReleaseTool.Policies;

namespace Mirabeau.DatabaseReleaseTool
{
    // Example commandline
    // Mirabeau.DatabaseReleaseTool.exe -d:C:\Projects\YourProject\Databases\ExampleDb -c:"C:\Projects\YourProject\Databases\ExampleDb\DevConfig.config" -vf:1.0 -vt:2.0.0.0
    internal class Program
    {
        #region Static Fields
        
        private static readonly IApplicationOptionsParser optionsParser = new ApplicationOptionsParser();

        private static readonly IMandatoryArgumentValidator argumentValidator = new MandatoryArgumentValidator(optionsParser);

        private static readonly IOutputLogger outputLogger = new ConsoleOutputLogger();

        #endregion

        #region Methods

        private static BeforeExecuteScriptsAction GetBeforeExecuteScriptsActionFromConfigurationFile(string configurationFileName)
        {
            if (!File.Exists(configurationFileName))
            {
                throw new FileNotFoundException(string.Format("Configurationfile: {0} not found.", configurationFileName));
            }

            System.Configuration.Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap { ExeConfigFilename = configurationFileName }, 
                    ConfigurationUserLevel.None);

            KeyValueConfigurationElement keyValueConfigurationElement = config.AppSettings.Settings["BeforeExecuteScriptsAction"];

            if (keyValueConfigurationElement != null && !string.IsNullOrWhiteSpace(keyValueConfigurationElement.Value)
                && keyValueConfigurationElement.Value.Equals("CreateDatabase", StringComparison.InvariantCultureIgnoreCase))
            {
                return BeforeExecuteScriptsAction.CreateDatabase;
            }

            return BeforeExecuteScriptsAction.None;
        }

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return -1;
            }

            // Initialize default values            
            ApplicationOptions options = optionsParser.Parse(args);

            if (options.InformationSwitch == InformationSwitch.On)
            {
                // Print extra info
                PrintUsage();
                PrintDatabaseDirectoryStructureTemplate();
                return -2;
            }

            ArgumentValidationResult validationResult = argumentValidator.Validate(args);

            if (validationResult.Status == ArgumentValidationStatus.Failed)
            {
                outputLogger.WriteErrorMessage("\nArgument error:\n");
                validationResult.ValidationMessages.ForEach(
                    item => outputLogger.WriteErrorMessage(string.Format(CultureInfo.InvariantCulture, "\t{0}\n", item)));
                PrintUsage();
                return -3;
            }

            DatabaseReleaseTool databaseReleaseTool;

            try
            {
                databaseReleaseTool = new DatabaseReleaseTool(
                    options.DatabaseScriptsPath, 
                    new CreateDatabasePolicyComposite(), 
                    new FileStructurePolicyComposite(), 
                    outputLogger, 
                    new ConnectionStringFactory(), 
                    new DatabaseConnectionFactory());
            }
            catch (Exception ex)
            {
                outputLogger.WriteErrorMessage(string.Format("Error while creating databasereleasetool. {0}.", ex.Message));
                outputLogger.WriteDebugMessage(ex.StackTrace);
                return -5;
            }

            DatabaseConnectionParameters connectionParameters = new DatabaseConnectionParameters();

            // Check if a configuration file parameter is passed.
            // If so then the database configuration will be read from the config file instead of the commandline.
            if (!string.IsNullOrEmpty(options.ConfigurationFilename))
            {
                // Check with configfile                                
                outputLogger.WriteInfoMessage("Starting DatabaseRelease Tool in ConfigFile Modus.");

                outputLogger.WriteDebugMessage(
                    string.Format(CultureInfo.InvariantCulture, "ConfigFile: {0}.", options.ConfigurationFilename));
                outputLogger.WriteDebugMessage(
                    string.Format(CultureInfo.InvariantCulture, "Database Path: {0}.", options.DatabaseScriptsPath));

                // Create databasesettings with configfile.                
                connectionParameters.CreationType = ConnectionStringCreationType.FromConfigurationFile;
                connectionParameters.ConfigurationFileName = options.ConfigurationFilename;
                connectionParameters.BeforeExecuteScriptsAction =
                    GetBeforeExecuteScriptsActionFromConfigurationFile(options.ConfigurationFilename);
            }
            else
            {
                // check with databaseparams                
                connectionParameters.CreationType = ConnectionStringCreationType.FromArguments;
                connectionParameters.BeforeExecuteScriptsAction = options.BeforeExecuteScriptsAction;
                connectionParameters.Arguments.Hostname = options.Servername;
                connectionParameters.Arguments.Database = options.DatabaseName;
                connectionParameters.Arguments.Username = options.Username;
                connectionParameters.Arguments.Password = options.Password;
            }

            // DatabaseType is given
            if (options.DatabaseType != DatabaseType.None)
            {
                DatabaseType type = options.DatabaseType;

                // database type is mssql by default.                    
                connectionParameters.DatabaseType = type;
                outputLogger.WriteInfoMessage(string.Format(CultureInfo.InvariantCulture, "DatabaseType is: {0}.", type));
            }
            else
            {
                connectionParameters.DatabaseType = DatabaseType.MsSql; // default;                

                // Create databasesettings with parameters.                    
                outputLogger.WriteInfoMessage("No DatabaseType was given using default MsSql.");
            }

            ExcecutionResult result;
            try
            {
                result = databaseReleaseTool.Execute(
                    options.FromVersion, 
                    options.ToVersion, 
                    connectionParameters, 
                    options.EncodingForReadingSqlScripts);
            }
            catch (Exception ex)
            {
                outputLogger.WriteErrorMessage(
                    string.Format(CultureInfo.InvariantCulture, "Unexpected error while Executing scripts. {0}.", ex.Message));
                outputLogger.WriteDebugMessage(ex.StackTrace);
                return -6;
            }

            if (!result.Success)
            {
                outputLogger.WriteErrorMessage("Excecution failed with the following errors: ");
                outputLogger.WriteErrorMessage(string.Join("\n", result.Errors.ToArray()));
                return -10;
            }

            outputLogger.WriteSuccessMessage("Execution succeeded.");

            return 0;
        }

        private static void PrintDatabaseDirectoryStructureTemplate()
        {
            outputLogger.WriteSuccessMessage("Template for the directory structure:");
            Console.Out.Write(Resources.DirectoryStructureTemplate);
            Console.Out.WriteLine();
            Console.Out.WriteLine();
            outputLogger.WriteSuccessMessage("Directory names that will be ignored:");
            Console.Out.Write(Resources.DirectoryIgnoreListStructureTemplate);
        }

        private static void PrintUsage()
        {
            Console.Out.Write(Resources.UsageTemplate);
        }

        #endregion
    }
}