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
        private const string ValuesLiteral = "values";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public InsertCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            InputValidator = validator;
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'insert' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
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
                PrintIncorrectSyntax(input);
                return false;
            }

            return true;
        }

        private static void Insert(string parameters)
        {
            BreakLastKeyValuePairsState(PropertiesNameValuePairs);
            bool result = TryFillPropertyPairs(parameters);
            if (!result)
            {
                return;
            }

            RecordArguments arguments = new RecordArguments();
            result = result && TryToUpdateArguments(arguments, PropertiesNameValuePairs);
            if (!result)
            {
                return;
            }

            FileCabinetRecord record = service.GetRecords().FirstOrDefault(r => r.Id == arguments.Id);

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

        private static bool TryFillPropertyPairs(string parameters)
        {
            const int ParametersNamesIndex = 0;
            const int ParametersValuesIndex = 1;
            bool isValid = parameters.Contains(ValuesLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (!isValid)
            {
                PrintIncorrectSyntax(parameters);
                return false;
            }

            var namesValuesPairs = parameters.Split(ValuesLiteral);
            isValid = namesValuesPairs.Length > 1;
            if (!isValid)
            {
                PrintIncorrectSyntax(parameters);
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
            isValid = TryFillPropetyPairs(parametersNames, parametersValues);
            if (!isValid)
            {
                return false;
            }

            return true;
        }

        private static bool TryFillPropetyPairs(string parametersNames, string parametersValues)
        {
            string[] names = parametersNames.Split(',');
            string[] values = parametersValues.Split(',');
            if (names.Length != values.Length)
            {
                return false;
            }

            bool result = true;
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Trim();
                bool isCorrectName = PropertiesNameValuePairs.ContainsKey(names[i]);
                bool isKvpValueNull = isCorrectName && PropertiesNameValuePairs[names[i]] is null;
                result &= isCorrectName && isKvpValueNull;
                if (result)
                {
                    PropertiesNameValuePairs[names[i]] = values[i].Trim().Trim('\'');
                }
                else
                {
                    if (!isCorrectName)
                    {
                        PrintIncorrectSyntax(parametersNames, names[i]);
                    }
                    else if (!isKvpValueNull)
                    {
                        PrintIncorrectSyntax(parametersValues, values[i]);
                    }

                    break;
                }
            }

            return result;
        }
    }
}
