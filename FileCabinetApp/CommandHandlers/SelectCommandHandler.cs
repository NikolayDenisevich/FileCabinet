using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'update' command handler for FileCabinetApp.
    /// </summary>
    internal class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string SelectCommand = "select";
        private const string WhereLiteral = "where ";

        private static Dictionary<string, List<object>> filtersDictionary;
        private static Dictionary<string, string> selectorsDictionary;
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        public SelectCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'update' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
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
            filtersDictionary = new Dictionary<string, List<object>>(StringComparer.InvariantCultureIgnoreCase);
            selectorsDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            InitializeDictionaryByPropertiesNames(selectorsDictionary);
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
            const string NeedMarker = "+";
            Parser.SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, NeedMarker);
            service.GetRecords(null, null).CreateTable(selectorsDictionary, Console.Out);
        }

        private static void SelectByCriteria(string parameters)
        {
            int selectorsIndex = 0;
            int filtersIndex = 1;
            bool result;
            bool areFiltersPresent = parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            bool areSelectorsPresent = false;
            bool isAndAlsoCombineMethod = false;
            string[] splittedParameters = null;
            string filters;
            if (areFiltersPresent)
            {
                splittedParameters = parameters.Split(WhereLiteral, 2, StringSplitOptions.RemoveEmptyEntries);
                areSelectorsPresent = splittedParameters.Length > 1;
                filtersIndex = areSelectorsPresent ? filtersIndex : selectorsIndex;
                filters = areSelectorsPresent ? splittedParameters[filtersIndex] : splittedParameters[filtersIndex];
                result = Parser.TryParceFilters(filters, filtersDictionary, out isAndAlsoCombineMethod);
                if (!result)
                {
                    return;
                }
            }

            if (areSelectorsPresent)
            {
                string selectors = splittedParameters is null ? parameters : splittedParameters[selectorsIndex];
                result = Parser.TryParceSelectors(selectors, selectorsDictionary);
                if (!result)
                {
                    return;
                }
            }
            else
            {
                const string NeedMarker = "+";
                Parser.SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, NeedMarker);
            }

            var filtersPredicate = PredicatesFactory.GetPredicate(filtersDictionary, isAndAlsoCombineMethod);
            filters = splittedParameters?[filtersIndex];
            IEnumerable<FileCabinetRecord> records = GetRecords(areFiltersPresent, filtersPredicate, filters);
            if (records is null)
            {
                return;
            }

            records.CreateTable(selectorsDictionary, Console.Out);
        }

        private static IEnumerable<FileCabinetRecord> GetRecords(bool areFiltersPresent, Func<FileCabinetRecord, bool> filtersPredicate, string filters)
        {
            IEnumerable<FileCabinetRecord> records;
            if (areFiltersPresent && filtersPredicate is null)
            {
                return null;
            }

            if (!areFiltersPresent)
            {
                records = service.GetRecords(null, null);
            }
            else
            {
                records = service.GetRecords(filters, filtersPredicate);
            }

            if (!records.Any())
            {
                Print.NoRecordsWithFilters(filters);
                records = null;
            }

            return records;
        }
    }
}
