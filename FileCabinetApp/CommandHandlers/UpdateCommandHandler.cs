using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'update' command handler for FileCabinetApp.
    /// </summary>
    internal class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string UpdateCommand = "update";
        private const string WhereLiteral = " where ";
        private const string SetLiteral = "set ";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;
        private static InputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when validator is null.</exception>
        public UpdateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            inputValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Handles the 'update' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(UpdateCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Update(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Update(string parameters)
        {
            const int ProperiesValuesIndex = 0;
            const int ProperiesFiltersIndex = 1;
            bool result = AreCorrectSetWhere(ref parameters);
            if (!result)
            {
                return;
            }

            string[] splittedParameters = parameters.Split(WhereLiteral, 2, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> settersDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(settersDictionary);
            result = Parser.TryParseUpdateSetters(splittedParameters[ProperiesValuesIndex], settersDictionary);
            if (!result)
            {
                return;
            }

            string filters = splittedParameters.Length > 1 ? splittedParameters[ProperiesFiltersIndex] : null;
            result = TryGetFilteredCollection(filters, service, out IEnumerable<FileCabinetRecord> records);
            if (!result || records is null)
            {
                return;
            }

            var recordsArray = records.ToArray();
            result = TryApplyChanges(recordsArray, settersDictionary);
            if (result)
            {
                Print.OperationResult(recordsArray, UpdateCommand);
            }
        }

        private static bool TryApplyChanges(IEnumerable<FileCabinetRecord> records, Dictionary<string, string> propertiesNameValuePairs)
        {
            bool result = false;
            foreach (var item in records)
            {
                RecordArguments arguments = GetRecordArguments(item);
                result = Input.TryToUpdateArgumentsWhithoutId(arguments, propertiesNameValuePairs, inputValidator);
                if (result)
                {
                    service.EditRecord(arguments);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private static RecordArguments GetRecordArguments(FileCabinetRecord record)
        {
            RecordArguments arguments = new RecordArguments();
            var argsProperties = typeof(RecordArguments).GetProperties();
            var recordProperties = typeof(FileCabinetRecord).GetProperties();
            for (int i = 0; i < argsProperties.Length; i++)
            {
                argsProperties[i].SetValue(arguments, recordProperties[i].GetValue(record));
            }

            return arguments;
        }

        private static bool AreCorrectSetWhere(ref string parameters)
        {
            const int CorrectSetIndex = 0;
            bool isEmpty = string.IsNullOrWhiteSpace(parameters);
            if (isEmpty)
            {
                Print.ParametrizedCommandHint();
                return false;
            }

            parameters = parameters.Trim();
            int setIndex = parameters.IndexOf(SetLiteral, StringComparison.InvariantCultureIgnoreCase);
            bool isCorrect = setIndex == CorrectSetIndex;
            if (!isCorrect)
            {
                Print.IncorrectSyntax(parameters);
                return false;
            }
            else
            {
                parameters = parameters.Replace(SetLiteral, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
            }

            isCorrect = parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (!isCorrect)
            {
                Print.IncorrectSyntax(parameters);
            }

            return isCorrect;
        }
    }
}
