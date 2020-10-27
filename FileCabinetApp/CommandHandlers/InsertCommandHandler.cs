using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'insert' command handler for FileCabinetApp.
    /// </summary>
    internal class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string InsertCommand = "insert";
        private const string ValuesLiteral = " values ";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;
        private static InputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when validator is null.</exception>
        public InsertCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            inputValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Handles the 'insert' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(InsertCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Insert(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static bool IsValidParenthesesPair(string input)
        {
            if (!input.StartsWith('(') || !input.EndsWith(')'))
            {
                Print.IncorrectSyntax(input);
                return false;
            }

            return true;
        }

        private static void Insert(string parameters)
        {
            Dictionary<string, string> selectorsDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(selectorsDictionary);
            Parser.SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, null);
            bool result = TryFillPropertyPairs(parameters, selectorsDictionary);
            if (!result)
            {
                return;
            }

            RecordArguments arguments = new RecordArguments();
            result = result && Input.TryToUpdateArgumentsWithId(arguments, selectorsDictionary, inputValidator);
            if (!result)
            {
                return;
            }

            FileCabinetRecord record = service.GetRecords($"id={arguments.Id}", r => r.Id == arguments.Id).FirstOrDefault();
            if (record is null)
            {
                int id = service.CreateRecord(arguments);
                Console.WriteLine($"Record #{id} is inserted");
            }
            else
            {
                Console.WriteLine($"Record #{arguments.Id} is allready exists");
            }
        }

        private static bool TryFillPropertyPairs(string parameters, Dictionary<string, string> selectorsDictionary)
        {
            const int ParametersNamesIndex = 0;
            const int ParametersValuesIndex = 1;
            bool isValid = parameters.Contains(ValuesLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (!isValid)
            {
                Print.IncorrectSyntax(parameters);
                return false;
            }

            var namesValuesPairs = parameters.Split(ValuesLiteral);
            isValid = namesValuesPairs.Length > 1;
            if (!isValid)
            {
                Print.IncorrectSyntax(parameters);
                return false;
            }

            string parametersNames = namesValuesPairs[ParametersNamesIndex].Trim();
            string parametersValues = namesValuesPairs[ParametersValuesIndex].Trim();
            isValid = IsValidParenthesesPair(parametersNames) && IsValidParenthesesPair(parametersValues);
            if (!isValid)
            {
                return false;
            }

            parametersNames = parametersNames.Trim('(', ')');
            parametersValues = parametersValues.Trim('(', ')');
            isValid = TryFillPropetyPairs(parametersNames, parametersValues, selectorsDictionary);
            if (!isValid)
            {
                return false;
            }

            return true;
        }

        private static bool TryFillPropetyPairs(string parametersNames, string parametersValues, Dictionary<string, string> selectorsDictionary)
        {
            const char PropertiesSeparator = ',';
            int validPropertiesCount = typeof(FileCabinetRecord).GetProperties().Length;
            string[] names = parametersNames.Split(PropertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            string[] values = parametersValues.Split(PropertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length != values.Length || names.Length != validPropertiesCount || values.Length != validPropertiesCount)
            {
                Print.MismatchOfParametersAndValues(parametersNames, parametersValues);
                return false;
            }

            bool result = true;
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Trim();
                bool isCorrectName = selectorsDictionary.ContainsKey(names[i]);
                bool isKvpValueNull = isCorrectName && selectorsDictionary[names[i]] is null;
                result &= isCorrectName && isKvpValueNull;
                if (result)
                {
                    selectorsDictionary[names[i]] = values[i].Trim().Trim('\'');
                }
                else
                {
                    if (!isCorrectName)
                    {
                        Print.IncorrectSyntax(parametersNames, names[i]);
                    }
                    else if (!isKvpValueNull)
                    {
                        Print.RepeatedProperty(names[i]);
                    }

                    break;
                }
            }

            return result;
        }
    }
}
