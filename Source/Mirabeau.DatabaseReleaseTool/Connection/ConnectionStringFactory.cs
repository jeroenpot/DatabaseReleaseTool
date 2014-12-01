using System;
using System.Configuration;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;

using Mirabeau.DatabaseReleaseTool.Arguments;

using MySql.Data.MySqlClient;

namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        #region Public Methods and Operators

        public string Create(DatabaseConnectionParameters parameters)
        {
            switch (parameters.CreationType)
            {
                case ConnectionStringCreationType.FromConfigurationFile:
                    return CreateFromConfigurationFile(parameters);
                case ConnectionStringCreationType.FromArguments:
                    return CreateFromArguments(parameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Methods

        private static string CreateFromArguments(DatabaseConnectionParameters parameters)
        {
            switch (parameters.DatabaseType)
            {
                case DatabaseType.MsSql:
                    SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                                                                            {
                                                                                DataSource =
                                                                                    parameters.Arguments
                                                                                    .Hostname, 
                                                                                UserID =
                                                                                    parameters.Arguments
                                                                                    .Username, 
                                                                                Password =
                                                                                    parameters.Arguments
                                                                                    .Password, 
                                                                                InitialCatalog =
                                                                                    parameters.Arguments
                                                                                    .Database
                                                                            };
                    if (parameters.BeforeExecuteScriptsAction == BeforeExecuteScriptsAction.CreateDatabase)
                    {
                        sqlConnectionStringBuilder.InitialCatalog = "master";
                    }

                    return sqlConnectionStringBuilder.ConnectionString;
                case DatabaseType.Oracle:
                    if (parameters.BeforeExecuteScriptsAction == BeforeExecuteScriptsAction.CreateDatabase)
                    {
                        throw new ConfigurationErrorsException("Create database is not allowed in combination with Oracle.");
                    }

                    return
#pragma warning disable 618
                        new OracleConnectionStringBuilder
#pragma warning restore 618
                        {
                            DataSource = parameters.Arguments.Hostname, 
                            UserID = parameters.Arguments.Username, 
                            Password = parameters.Arguments.Password, 
                        }.ConnectionString;
                case DatabaseType.MySql:
                    if (parameters.BeforeExecuteScriptsAction == BeforeExecuteScriptsAction.CreateDatabase)
                    {
                        throw new ConfigurationErrorsException("Create database is not allowed in combination with MySql.");
                    }

                    return
                        new MySqlConnectionStringBuilder
                        {
                            Server = parameters.Arguments.Hostname, 
                            UserID = parameters.Arguments.Username, 
                            Password = parameters.Arguments.Password, 
                            Database = parameters.Arguments.Database
                        }.ConnectionString;
                default:
                    throw new ArgumentOutOfRangeException("parameters.DatabaseType");
            }
        }

        private static string CreateFromConfigurationFile(DatabaseConnectionParameters parameters)
        {
            if (!File.Exists(parameters.ConfigurationFileName))
            {
                throw new FileNotFoundException(string.Format("Configurationfile: {0} not found.", parameters.ConfigurationFileName));
            }

            System.Configuration.Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap { ExeConfigFilename = parameters.ConfigurationFileName }, 
                    ConfigurationUserLevel.None);

            string connectionString =
                config.ConnectionStrings.ConnectionStrings["Mirabeau.DatabaseReleaseTool.ConnectionString"].ConnectionString;

            if (parameters.BeforeExecuteScriptsAction != BeforeExecuteScriptsAction.CreateDatabase)
            {
                return connectionString;
            }

            if (parameters.DatabaseType == DatabaseType.MsSql)
            {
                return new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" }.ConnectionString;
            }

            throw new ConfigurationErrorsException(
                string.Format("Create database is not allowed in combination with {0}.", parameters.DatabaseType));
        }

        #endregion
    }
}