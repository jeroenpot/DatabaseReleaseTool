using System.Text;

using Mirabeau.DatabaseReleaseTool.Arguments;

using NUnit.Framework;

namespace Mirabeau.DatabaseReleaseTool.UnitTests
{
    [TestFixture]
    public class ArgumentParsingTests
    {
        #region Fields

        private readonly IApplicationOptionsParser _applicationOptionsParser = new ApplicationOptionsParser();

        #endregion

        #region Public Methods and Operators

        [Test]
        public void ShouldParseArgumentsToApplicationOptionsWhenConfigFileIsGiven()
        {
            // arrange
            string[] argumentList = new string[4];
            argumentList[0] = CommandLineArgumentOptions.DatabasePathArgumentOption + "d:\\mypath\\directory";
            argumentList[1] = CommandLineArgumentOptions.FromDatabaseVersion + "1.0.0.0";
            argumentList[2] = CommandLineArgumentOptions.ToDatabaseVersion + "1.1.0.1";
            argumentList[3] = CommandLineArgumentOptions.ConfigFileArgumentOption + "db.config";

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.ConfigurationFilename, Is.EqualTo("db.config"));
        }

        [TestCase(CommandLineArgumentOptions.CreateDatabasergumentOption, BeforeExecuteScriptsAction.CreateDatabase)]
        [TestCase(null, BeforeExecuteScriptsAction.None)]
        [TestCase(" ", BeforeExecuteScriptsAction.None)]
        [TestCase("", BeforeExecuteScriptsAction.None)]
        public void ShouldParseCreateDatabasergumentOptionToApplicationOptionsWhenCreateDatabaseSwitchIsGiven(
            string commandLineArgumentOption, 
            BeforeExecuteScriptsAction expectedBeforeExecuteScriptsAction)
        {
            // arrange
            string[] argumentList = new string[1];
            argumentList[0] = commandLineArgumentOption;

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.BeforeExecuteScriptsAction, Is.EqualTo(expectedBeforeExecuteScriptsAction));
        }

        [Test]
        public void ShouldParseInfoArgumentToApplicationOptionsWhenInformationSwitchIsGiven()
        {
            // arrange
            string[] argumentList = new string[1];
            argumentList[0] = CommandLineArgumentOptions.InfoArgumentOption;

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.InformationSwitch, Is.EqualTo(InformationSwitch.On));
        }

        [Test]
        public void ShouldParseStringArrayToApplicationOptions()
        {
            // arrange            
            string[] argumentList = CreateCompleteArgumentTestList();

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.DatabaseScriptsPath, Is.EqualTo("d:\\mypath\\directory"));
            Assert.That(applicationOptions.Username, Is.EqualTo("testuser"));
            Assert.That(applicationOptions.Password, Is.EqualTo("testpassword"));
            Assert.That(applicationOptions.Servername, Is.EqualTo("localhost"));
            Assert.That(applicationOptions.DatabaseType, Is.EqualTo(DatabaseType.MySql));
            Assert.That(applicationOptions.FromVersion, Is.EqualTo("1.0.0.0"));
            Assert.That(applicationOptions.ToVersion, Is.EqualTo("1.1.0.1"));
            Assert.That(applicationOptions.DatabaseName, Is.EqualTo("TestDb"));
            Assert.That(applicationOptions.InformationSwitch, Is.EqualTo(InformationSwitch.None));
        }

        [Test]
        public void ShouldReturnDefaultEncodingWhenNoEncodingIsSpecified()
        {
            // arrange
            string[] argumentList = new string[0];

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.EncodingForReadingSqlScripts, Is.EqualTo(Encoding.Default));
        }

        [Test]
        public void ShouldReturnNullWhenEmptyArgumentListIsGiven()
        {
            // arrange
            string[] arguments = new string[1];

            // act
            string result = ArgumentParsingHelper.GetArgumentValueFromArguments(arguments, "parameter:");

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ShouldReturnStatusValidWhenValidOptionsForConfigFileAreGiven()
        {
            // Arrange
            string[] argumentList = new string[4];
            argumentList[0] = CommandLineArgumentOptions.DatabasePathArgumentOption + "d:\\mypath\\directory";
            argumentList[1] = CommandLineArgumentOptions.FromDatabaseVersion + "1.0.0.0";
            argumentList[2] = CommandLineArgumentOptions.ToDatabaseVersion + "1.1.0.1";
            argumentList[3] = CommandLineArgumentOptions.ConfigFileArgumentOption + "db.config";

            IMandatoryArgumentValidator validator = new MandatoryArgumentValidator(_applicationOptionsParser);

            // Act
            ArgumentValidationResult argumentValidationResult = validator.Validate(argumentList);

            // Assert
            Assert.That(argumentValidationResult.Status, Is.EqualTo(ArgumentValidationStatus.Valid));
        }

        [Test]
        public void ShouldReturnUtf8EncodingWhenSpecified()
        {
            // arrange
            string[] argumentList = new string[1];
            argumentList[0] = CommandLineArgumentOptions.EncodingUtf8;

            // act
            ApplicationOptions applicationOptions = _applicationOptionsParser.Parse(argumentList);

            // assert
            Assert.That(applicationOptions.EncodingForReadingSqlScripts, Is.EqualTo(Encoding.UTF8));
        }

        [Test]
        public void ShouldReturnValidationFailReasonListWhenValidationFails()
        {
            // arrange
            string[] argumentList = new string[1];
            argumentList[0] = "invalidArgument: testValue";
            IMandatoryArgumentValidator validator = new MandatoryArgumentValidator(_applicationOptionsParser);

            // act
            ArgumentValidationResult argumentValidationResult = validator.Validate(argumentList);

            // assert
            Assert.That(argumentValidationResult.ValidationMessages[0], Is.EqualTo("Database script path was not given as an argument."));
        }

        [Test]
        public void ShouldReturnValidationFailReasonListWhenValidationFailsWithOneInvalidArgument()
        {
            // arrange
            string[] argumentList = CreateCompleteArgumentTestList();
            argumentList[3] = "invalidArgument: testvalue";
            IMandatoryArgumentValidator validator = new MandatoryArgumentValidator(_applicationOptionsParser);

            // act
            ArgumentValidationResult argumentValidationResult = validator.Validate(argumentList);

            // assert
            Assert.That(argumentValidationResult.ValidationMessages[0], Is.EqualTo("Invalid argument(s) passed."));
        }

        [Test]
        public void ShouldReturnValidationStatusFailedWhenEmpyArgumentsAreGiven()
        {
            // arrange
            string[] argumentList = new string[1];
            IMandatoryArgumentValidator validator = new MandatoryArgumentValidator(_applicationOptionsParser);

            // act
            ArgumentValidationResult argumentValidationResult = validator.Validate(argumentList);

            // assert
            Assert.That(argumentValidationResult.Status, Is.EqualTo(ArgumentValidationStatus.Failed));
        }

        [Test]
        public void ShouldReturnValueAfterGivenOptionFromStringArrayWithOneItem()
        {
            // arrange
            string[] arguments = new string[1];
            arguments[0] = "parameter: valueToReturn";

            // act
            string result = ArgumentParsingHelper.GetArgumentValueFromArguments(arguments, "parameter:");

            // assert
            Assert.That(result, Is.EqualTo(" valueToReturn"));
        }

        [Test]
        public void ShouldReturnValueAfterGivenOptionFromStringArrayWithTwoItems()
        {
            // arrange
            string[] arguments = new string[2];
            arguments[0] = "parameter: valueToReturn";
            arguments[0] = "secondParam: valueToReturnTwo";

            // act
            string result = ArgumentParsingHelper.GetArgumentValueFromArguments(arguments, "secondParam:");

            // assert
            Assert.That(result, Is.EqualTo(" valueToReturnTwo"));
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenGivenKeyIsNotFound()
        {
            // arrange
            string[] arguments = new string[1];
            arguments[0] = "notgiven: valueToReturn";

            // act
            string result = ArgumentParsingHelper.GetArgumentValueFromArguments(arguments, "parameter:");

            // assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Methods

        private static string[] CreateCompleteArgumentTestList()
        {
            string[] argumentList = new string[8];
            argumentList[0] = CommandLineArgumentOptions.DatabasePathArgumentOption + "d:\\mypath\\directory";
            argumentList[1] = CommandLineArgumentOptions.DatabasenameArgumentOption + "TestDb";
            argumentList[2] = CommandLineArgumentOptions.UsernameArgumentOption + "testuser";
            argumentList[3] = CommandLineArgumentOptions.PasswordArgumentOption + "testpassword";
            argumentList[4] = CommandLineArgumentOptions.ServernameArgumentOption + "localhost";
            argumentList[5] = CommandLineArgumentOptions.DatabasetypeArgumentOption + "MySql";
            argumentList[6] = CommandLineArgumentOptions.FromDatabaseVersion + "1.0.0.0";
            argumentList[7] = CommandLineArgumentOptions.ToDatabaseVersion + "1.1.0.1";
            return argumentList;
        }

        #endregion
    }
}