namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public interface IMandatoryArgumentValidator
    {
        #region Public Methods and Operators

        ArgumentValidationResult Validate(string[] argumentList);

        #endregion
    }
}