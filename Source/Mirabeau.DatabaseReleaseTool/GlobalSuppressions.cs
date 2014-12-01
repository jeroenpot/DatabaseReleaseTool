using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteErrorMessage(System.String)", Scope = "member", 
        Target = "Mirabeau.DatabaseReleaseTool.Program.#Main(System.String[])",
        Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", 
        Target =
            "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutor.#ExecuteOneFileInCommand(System.String,System.Data.IDbTransaction,System.Text.Encoding)",
        Justification = "This is by design. The databaserelease tool, should be able to execute plain sql.")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Scope = "type", 
        Target = "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutorRollbackException", Justification = "No need to implement this now.")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteInfoLine(System.String,System.Int32)", Scope = "member", 
        Target =
            "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutor.#ExecuteOneFileInCommand(System.String,System.Data.IDbTransaction,System.Text.Encoding)",
        Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteInfoLine(System.String,System.Int32)", Scope = "member", 
        Target =
            "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutor.#ExcecuteAllSqlFilesInTransaction(System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding)",
             Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteInfoMessage(System.String)", Scope = "member", 
        Target = "Mirabeau.DatabaseReleaseTool.Program.#Main(System.String[])",
         Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteDebugMessage(System.String)", Scope = "member", 
        Target = "Mirabeau.DatabaseReleaseTool.Program.#Main(System.String[])",
        Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteSuccessMessage(System.String)", Scope = "member", 
        Target = "Mirabeau.DatabaseReleaseTool.Program.#Main(System.String[])",
         Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteSuccessMessage(System.String)", Scope = "member", 
        Target = "Mirabeau.DatabaseReleaseTool.Program.#PrintDatabaseDirectoryStructureTemplate()",
         Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
        MessageId = "Mirabeau.DatabaseReleaseTool.Logging.IOutputLogger.WriteInfoLine(System.String,System.Int32)", Scope = "member", 
        Target =
            "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutor.#ExcecuteAllSqlFiles(System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding)",
         Justification = "Literals are used for output logging")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", 
        Target =
            "Mirabeau.DatabaseReleaseTool.Sql.TransactionalSqlFileExecutor.#ExcecuteAllSqlFiles(System.Collections.Generic.IEnumerable`1<System.String>,System.Text.Encoding)",
            Justification = "This is by design. The databaserelease tool, should be able to execute plain sql.")]