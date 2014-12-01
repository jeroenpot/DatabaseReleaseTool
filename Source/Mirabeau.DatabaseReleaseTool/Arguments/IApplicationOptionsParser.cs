namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public interface IApplicationOptionsParser
    {
        #region Public Methods and Operators

        ApplicationOptions Parse(string[] argumentList);

        #endregion
    }
}