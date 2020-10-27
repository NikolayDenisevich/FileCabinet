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

        private static Dictionary<string, List<object>> filtersDictionary;
        private static Dictionary<string, string> settersDictionary;

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;
        private static InputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public UpdateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            service = fileCabinetService;
            inputValidator = validator;
        }

        /// <summary>
        /// Handles the 'update' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
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
            filtersDictionary = new Dictionary<string, List<object>>(StringComparer.InvariantCultureIgnoreCase);
            settersDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(settersDictionary);
            const int ProperiesValuesIndex = 0;
            const int ProperiesFiltersIndex = 1;
            bool result = AreCorrectSetWhere(ref parameters);
            if (!result)
            {
                return;
            }

            string[] splittedParameters = parameters.Split(WhereLiteral, 2, StringSplitOptions.RemoveEmptyEntries);
            string filters = splittedParameters.Length > 1 ? splittedParameters[ProperiesFiltersIndex] : null;
            result = Parser.TryParceFilters(filters, filtersDictionary, out bool isAndAlsoCombineMethod);
            if (!result)
            {
                return;
            }

            result = Parser.TryParceUpdateSetters(splittedParameters[ProperiesValuesIndex], settersDictionary);
            if (!result)
            {
                return;
            }

            var filtersPredicate = PredicatesFactory.GetPredicate(filtersDictionary, isAndAlsoCombineMethod);
            if (filtersPredicate is null)
            {
                return;
            }

            var records = service.GetRecords(parameters, filtersPredicate);
            if (!records.Any())
            {
                Print.NoRecordsWithFilters(filters);
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
                result = Input.TryToUpdateArguments(arguments, propertiesNameValuePairs, inputValidator);
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
