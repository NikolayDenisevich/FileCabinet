using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'update' command handler for FileCabinetApp.
    /// </summary>
    internal class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string SelectCommand = "select";
        private const string WhereLiteral = "where ";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public SelectCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Handles the 'update' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(SelectCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Select(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Select(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                SelectAll();
            }
            else
            {
                SelectByCriteria(parameters);
            }
        }

        private static void SelectAll()
        {
            Dictionary<string, string> selectorsDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(selectorsDictionary);
            const string NeedMarker = "+";
            Parser.SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, NeedMarker);
            SelectWhithoutFiltersUsingSelectors(selectorsDictionary);
        }

        private static void SelectWhithoutFiltersUsingSelectors(Dictionary<string, string> selectorsDictionary)
        {
            var records = service.GetRecords(null, null);
            if (records.Any())
            {
                records.CreateTable(selectorsDictionary, Console.Out);
            }
            else
            {
                Print.NoRecords();
            }
        }

        private static void SelectByCriteria(string parameters)
        {
            Dictionary<string, string> selectorsDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(selectorsDictionary);
            bool areFiltersPresent = parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (areFiltersPresent)
            {
                SelectWithFiltersAndSelectors(parameters, selectorsDictionary);
            }
            else
            {
                bool result = Parser.TryParseSelectors(parameters, selectorsDictionary);
                if (!result)
                {
                    return;
                }

                SelectWhithoutFiltersUsingSelectors(selectorsDictionary);
            }
        }

        private static void SelectWithFiltersAndSelectors(string parameters, Dictionary<string, string> selectorsDictionary)
        {
            int selectorsIndex = 0;
            int filtersIndex = 1;
            bool result;
            string[] splittedParameters = parameters.Split(WhereLiteral, 2, StringSplitOptions.RemoveEmptyEntries);
            bool areSelectorsPresent = splittedParameters.Length > 1;
            filtersIndex = areSelectorsPresent ? filtersIndex : selectorsIndex;
            string filters = splittedParameters[filtersIndex];
            string selectors = areSelectorsPresent ? splittedParameters[selectorsIndex] : null;
            result = Parser.TryParseSelectors(selectors, selectorsDictionary);
            if (!result)
            {
                return;
            }

            result = TryGetFilteredCollection(filters, service, out IEnumerable<FileCabinetRecord> records);
            if (!result)
            {
                return;
            }

            records.CreateTable(selectorsDictionary, Console.Out);
        }
    }
}
