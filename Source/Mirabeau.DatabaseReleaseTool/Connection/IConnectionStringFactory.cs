namespace Mirabeau.DatabaseReleaseTool.Connection
{
    public interface IConnectionStringFactory
    {
        #region Public Methods and Operators

        string Create(DatabaseConnectionParameters parameters);

        #endregion
    }
}