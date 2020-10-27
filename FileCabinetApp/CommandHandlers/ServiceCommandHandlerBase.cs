using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides CommandHandler base class.
    /// </summary>
    internal class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Adds to the dictionary keys as names of FileCabinetRecord properties and null values.
        /// </summary>
        /// <param name="dictionary">Target dictionary.</param>
        /// <exception cref="ArgumentNullException">Thrown when dictionary is null.</exception>
        internal static void InitializeDictionaryByPropertiesNames(Dictionary<string, string> dictionary)
        {
            dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            var properties = typeof(FileCabinetRecord).GetProperties();
            foreach (var property in properties)
            {
                dictionary.Add(property.Name, null);
            }
        }

        /// <summary>
        /// Gets filtered collection using specified filters. A return value indicates whether the collection creation result is succeeded.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="service">FileCabinetService instance.</param>
        /// <param name="collection">When this method returns, contains records collection filtered by specified filters, if the creation sucseeded, or null, if the creation failed or there is no records found using specified filters.</param>
        /// <returns>true if collection was created successfully; otherwise, false.</returns>
        internal static bool TryGetFilteredCollection(string filters, IFileCabinetService<FileCabinetRecord, RecordArguments> service, out IEnumerable<FileCabinetRecord> collection)
        {
            bool result;
            collection = null;
            var filtersDictionary = new Dictionary<string, List<object>>(StringComparer.InvariantCultureIgnoreCase);
            result = Parser.TryParseFilters(filters, filtersDictionary, out bool isAndAlsoCombineMethod);
            if (!result)
            {
                return result;
            }

            var filtersPredicate = PredicatesFactory.GetPredicate(filtersDictionary, isAndAlsoCombineMethod);
            result = filtersPredicate != null;
            if (!result)
            {
                return result;
            }

            collection = service.GetRecords(filters, filtersPredicate);
            if (!collection.Any())
            {
                Print.NoRecordsWithFilters(filters);
                collection = null;
            }

            return result;
        }
    }
}
