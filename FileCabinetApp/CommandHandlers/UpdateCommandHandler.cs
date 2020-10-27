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
        private const string WhereLiteral = "where";
        private const string SetLiteral = "set";
        private const string AndLiteral = "and";

        private static readonly Dictionary<string, string> FiltersDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { IdName, null },
            { FirstNameName, null },
            { LastNameName, null },
            { DateOfBirthName, null },
            { ZipCodeName, null },
            { CityName, null },
            { StreetName, null },
            { SalaryName, null },
            { GenderName, null },
        };

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public UpdateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            service = fileCabinetService;
            InputValidator = validator;
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
            const int ProperiesValuesIndex = 0;
            const int ProperiesFiltersIndex = 1;
            bool result = AreCorrectSetWhere(ref parameters);
            if (!result)
            {
                return;
            }

            string[] splittedParameters = parameters.Split(WhereLiteral, 2, StringSplitOptions.RemoveEmptyEntries);
            result = TryParceFilters(splittedParameters[ProperiesFiltersIndex]);
            if (!result)
            {
                return;
            }

            result = TryParceSetters(splittedParameters[ProperiesValuesIndex]);
            if (!result)
            {
                return;
            }

            var predicate = GetPredicate(FiltersDictionary);
            if (predicate is null)
            {
                return;
            }

            IEnumerable<FileCabinetRecord> records = service.GetRecords().Where(predicate);
            if (!records.Any())
            {
                PrintNoRecordsWithFilters(splittedParameters[ProperiesFiltersIndex]);
                return;
            }

            result = TryApplyChanges(records, PropertiesNameValuePairs);
            if (result)
            {
                PrintOperationResult(records, UpdateCommand);
            }
        }

        private static bool TryApplyChanges(IEnumerable<FileCabinetRecord> records, Dictionary<string, string> propertiesNameValuePairs)
        {
            bool result = false;
            foreach (var item in records)
            {
                RecordArguments arguments = GetRecordArguments(item);
                result = TryToUpdateArguments(arguments, propertiesNameValuePairs);
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
            arguments.Id = record.Id;
            arguments.FirstName = record.FirstName;
            arguments.LastName = record.LastName;
            arguments.DateOfBirth = record.DateOfBirth;
            arguments.ZipCode = record.ZipCode;
            arguments.City = record.City;
            arguments.Street = record.Street;
            arguments.Salary = record.Salary;
            arguments.Gender = record.Gender;
            return arguments;
        }

        private static bool TryParceSetters(string setters)
        {
            const string CommaPropertiesSeparator = ",";
            BreakLastKeyValuePairsState(PropertiesNameValuePairs);
            setters = setters.Trim();
            if (string.IsNullOrWhiteSpace(setters))
            {
                PrintIncorrectSyntax(setters);
                return false;
            }

            if (setters.Contains(IdName, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintCannotSetValueToProperty(IdName);
                return false;
            }

            return TryParceSetOfNameValuePairs(setters, CommaPropertiesSeparator, PropertiesNameValuePairs);
        }

        private static Func<FileCabinetRecord, bool> GetPredicate(Dictionary<string, string> filtersDictionary)
        {
            var expressionsList = new List<Func<FileCabinetRecord, bool>>();
            foreach (var item in filtersDictionary)
            {
                if (item.Value is null)
                {
                    continue;
                }

                Func<FileCabinetRecord, bool> expression = GetExpression(item.Key, item.Value);
                expressionsList.Add(expression);
            }

            Func<FileCabinetRecord, bool> predicate;
            if (expressionsList.Count == 0)
            {
                predicate = null;
            }
            else if (expressionsList.Count == 1)
            {
                predicate = expressionsList[0];
            }
            else
            {
                predicate = CombinePredicatesWithAnd(expressionsList);
            }

            return predicate;
        }

        private static Func<FileCabinetRecord, bool> GetExpression(string propertyName, string value)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            string propertyInLower = propertyName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            switch (propertyName)
            {
                case IdName:
                    {
                        bool isCorrect = TryCheckInput(value, IntConverter, InputValidator.ValidateInt, out int id);
                        Func<FileCabinetRecord, bool> expr = r => r.Id == id;
                        return isCorrect ? expr : null;
                    }

                case FirstNameName:
                    {
                        bool isCorrect = TryCheckInput(value, StringsConverter, InputValidator.ValidateStrings, out string firstName);
                        Func<FileCabinetRecord, bool> expr = r => r.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase);
                        return isCorrect ? expr : null;
                    }

                case LastNameName:
                    {
                        bool isCorrect = TryCheckInput(value, StringsConverter, InputValidator.ValidateStrings, out string lastName);
                        Func<FileCabinetRecord, bool> expr = r => r.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);
                        return isCorrect ? expr : null;
                    }

                case DateOfBirthName:
                    {
                        bool isCorrect = TryCheckInput(value, DatesConverter, InputValidator.ValidateDate, out DateTime date);
                        Func<FileCabinetRecord, bool> expr = r => r.DateOfBirth == date;
                        return isCorrect ? expr : null;
                    }

                case ZipCodeName:
                    {
                        bool isCorrect = TryCheckInput(value, ShortConverter, InputValidator.ValidateShort, out short zipCode);
                        Func<FileCabinetRecord, bool> expr = r => r.ZipCode == zipCode;
                        return isCorrect ? expr : null;
                    }

                case CityName:
                    {
                        bool isCorrect = TryCheckInput(value, StringsConverter, InputValidator.ValidateStrings, out string city);
                        Func<FileCabinetRecord, bool> expr = r => r.City.Equals(city, StringComparison.InvariantCultureIgnoreCase);
                        return isCorrect ? expr : null;
                    }

                case StreetName:
                    {
                        bool isCorrect = TryCheckInput(value, StringsConverter, InputValidator.ValidateStrings, out string street);
                        Func<FileCabinetRecord, bool> expr = r => r.Street.Equals(street, StringComparison.InvariantCultureIgnoreCase);
                        return isCorrect ? expr : null;
                    }

                case SalaryName:
                    {
                        bool isCorrect = TryCheckInput(value, DecimalConverter, InputValidator.ValidateDecimal, out decimal salary);
                        Func<FileCabinetRecord, bool> expr = r => r.Salary == salary;
                        return isCorrect ? expr : null;
                    }

                case GenderName:
                    {
                        bool isCorrect = TryCheckInput(value, CharConverter, InputValidator.ValidateChar, out char gender);
                        Func<FileCabinetRecord, bool> expr = r => r.Gender == gender;
                        return isCorrect ? expr : null;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        private static Func<T, bool> CombinePredicatesWithAnd<T>(List<Func<T, bool>> predicates)
        {
            int predicatesCount = predicates.Count;

            if (predicatesCount == 0)
            {
                return null;
            }

            Func<T, bool> predicateToReturn = predicates[0];

            if (predicatesCount == 1)
            {
                return predicateToReturn;
            }

            return (T item) =>
            {
                bool result = true;
                for (int i = 0; i < predicatesCount; i++)
                {
                    predicates[i] = predicates[i] ?? throw new ArgumentNullException($"{nameof(predicates)}[{i}]");
                    result &= predicates[i](item);
                }

                return result;
            };
        }

        private static bool TryParceFilters(string filters)
        {
            BreakLastKeyValuePairsState(FiltersDictionary);
            filters = filters.Trim();
            if (string.IsNullOrWhiteSpace(filters))
            {
                PrintIncorrectSyntax(filters);
                return false;
            }

            if (filters.Contains(AndLiteral, StringComparison.InvariantCultureIgnoreCase))
            {
                return TryParceSetOfNameValuePairs(filters, AndLiteral, FiltersDictionary);
            }
            else
            {
                return TryParceOneNameValuePair(filters, FiltersDictionary);
            }
        }

        private static bool TryParceSetOfNameValuePairs(string filters, string propertiesSeparator, Dictionary<string, string> destination)
        {
            string[] filtersKeyValuePairs = filters.Split(propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            foreach (var item in filtersKeyValuePairs)
            {
                parceResult = TryParceOneNameValuePair(item, destination);
                if (!parceResult)
                {
                    break;
                }
            }

            return parceResult;
        }

        // Для одной пары ключ-значение
        private static bool TryParceOneNameValuePair(string nameValuePair, Dictionary<string, string> destination)
        {
            nameValuePair = nameValuePair.Trim();
            if (string.IsNullOrWhiteSpace(nameValuePair))
            {
                PrintIncorrectSyntax(nameValuePair);
                return false;
            }

            var splittedPair = nameValuePair.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if ((splittedPair.Length & 1) == 1 || splittedPair.Length == 0)
            {
                PrintIncorrectSyntax(nameValuePair);
                return false;
            }

            for (int i = 0; i < splittedPair.Length; i++)
            {
                splittedPair[i] = splittedPair[i].Trim().Trim('\'');
            }

            return TryAddToPropertiesNameValuePairs(splittedPair, nameValuePair, destination);
        }

        // Для одной пары ключ-значение
        private static bool TryAddToPropertiesNameValuePairs(string[] splittedPair, string filterKeyValuePair, Dictionary<string, string> destination)
        {
            const int ValidPairLength = 2;
            const int KeyIndex = 0;
            const int ValueIndex = 1;
            if (splittedPair.Length != ValidPairLength)
            {
                PrintIncorrectSyntax(filterKeyValuePair);
                return false;
            }

            if (!destination.ContainsKey(splittedPair[KeyIndex]))
            {
                PrintIncorrectSyntax(splittedPair[KeyIndex]);
                return false;
            }

            if (destination[splittedPair[KeyIndex]] != null)
            {
                PrintRepeatedProperty(splittedPair[KeyIndex]);
                return false;
            }

            destination[splittedPair[KeyIndex]] = splittedPair[ValueIndex];
            return true;
        }

        private static void PrintRepeatedProperty(string propertyName)
        {
            Console.WriteLine($"Property '{propertyName}' already defined.");
        }

        private static bool AreCorrectSetWhere(ref string parameters)
        {
            const int CorrectSetIndex = 0;
            bool isEmpty = string.IsNullOrWhiteSpace(parameters);
            if (isEmpty)
            {
                PrintParametrizedCommandHint();
                return false;
            }

            parameters = parameters.Trim();
            int setIndex = parameters.IndexOf(SetLiteral, StringComparison.InvariantCultureIgnoreCase);
            bool isCorrect = setIndex == CorrectSetIndex;
            if (!isCorrect)
            {
                PrintIncorrectSyntax(parameters);
                return false;
            }
            else
            {
                parameters = parameters.Replace(SetLiteral, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
            }

            isCorrect = parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            return isCorrect;
        }

        private static void PrintNoRecordsWithFilters(string filters)
        {
            Console.WriteLine($"No records found using the specified filter(s) '{filters}'");
        }

        private static void PrintCannotSetValueToProperty(string propertyName)
        {
            Console.WriteLine($"Sorry, but you cannot set value to {propertyName}");
        }
    }
}
