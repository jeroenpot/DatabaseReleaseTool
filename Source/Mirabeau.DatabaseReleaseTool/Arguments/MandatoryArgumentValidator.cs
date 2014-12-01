namespace Mirabeau.DatabaseReleaseTool.Arguments
{
    public class MandatoryArgumentValidator : IMandatoryArgumentValidator
    {
        #region Fields

        private readonly IApplicationOptionsParser applicationOptionsParser;

        #endregion

        #region Constructors and Destructors

        public MandatoryArgumentValidator(IApplicationOptionsParser applicationOptionsParser)
        {
            this.applicationOptionsParser = applicationOptionsParser;
        }

        #endregion

        #region Public Methods and Operators

        public ArgumentValidationResult Validate(string[] argumentList)
        {
            ApplicationOptions options = applicationOptionsParser.Parse(argumentList);

            ArgumentValidationResult result = ValidateMinimalRequiredOptions(options);

            if (result.Status == ArgumentValidationStatus.Failed)
            {
                return result;
            }

            if (!string.IsNullOrEmpty(options.ConfigurationFilename))
            {
                return result;
            }

            if (argumentList.Length > 0 && !RequiredCommandlineOptionsModeAreSet(options))
            {
                result.Status = ArgumentValidationStatus.Failed;
                result.ValidationMessages.Add("Invalid argument(s) passed.");
            }

            return result;
        }

        #endregion

        #region Methods

        private static bool RequiredCommandlineOptionsModeAreSet(ApplicationOptions options)
        {
            return string.IsNullOrEmpty(options.DatabaseScriptsPath) == false && string.IsNullOrEmpty(options.Username) == false
                   && string.IsNullOrEmpty(options.Password) == false && string.IsNullOrEmpty(options.FromVersion) == false
                   && string.IsNullOrEmpty(options.ToVersion) == false && string.IsNullOrEmpty(options.DatabaseName) == false
                   && string.IsNullOrEmpty(options.Servername) == false;
        }

        private ArgumentValidationResult ValidateMinimalRequiredOptions(ApplicationOptions options)
        {
            ArgumentValidationResult result = new ArgumentValidationResult();
            result.Status = ArgumentValidationStatus.Valid;

            if (string.IsNullOrEmpty(options.DatabaseScriptsPath))
            {
                result.Status = ArgumentValidationStatus.Failed;
                result.ValidationMessages.Add("Database script path was not given as an argument.");
            }

            if (string.IsNullOrEmpty(options.FromVersion))
            {
                result.Status = ArgumentValidationStatus.Failed;
                result.ValidationMessages.Add("FromVersion script path was not given as an argument.");
            }

            if (string.IsNullOrEmpty(options.ToVersion))
            {
                result.Status = ArgumentValidationStatus.Failed;
                result.ValidationMessages.Add("ToVersion script path was not given as an argument.");
            }

            return result;
        }

        #endregion
    }
}