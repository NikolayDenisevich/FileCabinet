using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes Parser class.
    /// </summary>
    internal static class Parser
    {
        private const string WhereLiteral = "where ";
        private const string AndLiteral = " and ";
        private const string OrLiteral = " or ";
        private static InputValidator inputValidator;

        /// <summary>
        /// Gets or sets Input validator instance.
        /// </summary>
        /// <value>
        /// Input validator instance.
        /// </value>
        /// <exception cref="FieldAccessException">Thrown when inputValidator field is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        internal static InputValidator InputValidator
        {
            get => inputValidator ?? throw new FieldAccessException($"{nameof(inputValidator)} field is null.");
            set => inputValidator = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Parses selectors to the selectors dictionary for the select command.
        /// </summary>
        /// <param name="selectors">The string representation of the selectors.</param>
        /// <param name="selectorsDictionary">Parsing destinathon.</param>
        /// <returns>true if the selectors dictionary is filled successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when selectorsDictionary is null.</exception>
        internal static bool TryParseSelectors(string selectors, Dictionary<string, string> selectorsDictionary)
        {
            selectorsDictionary = selectorsDictionary ?? throw new ArgumentNullException(nameof(selectorsDictionary));
            const string SelectorsMarker = "+";
            if (string.IsNullOrWhiteSpace(selectors))
            {
                SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, SelectorsMarker);
                return true;
            }

            const string CommaPropertiesSeparator = ",";
            SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, null);
            selectors = selectors.Trim();
            return TryParseSetOfSelectors(selectors, CommaPropertiesSeparator, selectorsDictionary);
        }

        /// <summary>
        /// Parses setters to the setters dictionary for the update command.
        /// </summary>
        /// <param name="setters">The string representation of the setters.</param>
        /// <param name="settersDictionary">Parsing destination.</param>
        /// <returns>true if the setters dictionary is filled successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when settersDictionary is null.</exception>
        internal static bool TryParseUpdateSetters(string setters, Dictionary<string, string> settersDictionary)
        {
            settersDictionary = settersDictionary ?? throw new ArgumentNullException(nameof(settersDictionary));
            const string CommaPropertiesSeparator = ",";
            SetValueToAllDictionaryEntries<FileCabinetRecord>(settersDictionary, null);
            setters = setters.Trim();
            if (string.IsNullOrWhiteSpace(setters))
            {
                Print.IncorrectSyntax(setters);
                return false;
            }

            if (setters.Contains(nameof(FileCabinetRecord.Id), StringComparison.InvariantCultureIgnoreCase))
            {
                Print.CannotSetValueToProperty(nameof(FileCabinetRecord.Id));
                return false;
            }

            return TryParseSetOfUpdateSetters(setters, CommaPropertiesSeparator, settersDictionary);
        }

        /// <summary>
        /// Sets all propertiesNamesValuePairs dictionary values ​​to the specified value.
        /// </summary>
        /// <param name="propertiesNamesValuePairs">Name-Value pairs to set to the specified value.</param>
        /// <param name="value">Value to set to all dictionary entries.</param>
        /// <typeparam name="TRecord">Record type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when propertiesNamesValuePairs is null.</exception>
        internal static void SetValueToAllDictionaryEntries<TRecord>(Dictionary<string, string> propertiesNamesValuePairs, string value)
        {
            propertiesNamesValuePairs = propertiesNamesValuePairs ?? throw new ArgumentNullException(nameof(propertiesNamesValuePairs));
            var properties = typeof(TRecord).GetProperties();
            foreach (var property in properties)
            {
                if (propertiesNamesValuePairs.ContainsKey(property.Name))
                {
                    propertiesNamesValuePairs[property.Name] = value;
                }
            }
        }

        /// <summary>
        /// Parses command parameters.
        /// </summary>
        /// <param name="properties">A set of defined propeties.</param>
        /// <param name="parameters">Parameters for parsing.</param>
        /// <param name="propertyValue">When this method returns, contains parsed propertie value.</param>
        /// <returns>true if parameters parsed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when properties is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when parameters is null.</exception>
        internal static bool TryParseImportExportParameters(string[] properties, ref string parameters, out string propertyValue)
        {
            properties = properties ?? throw new ArgumentNullException(nameof(properties));
            parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            const char CharSeparator = ' ';
            string[] inputs = parameters.Split(CharSeparator, 2);
            const int propertyIndex = 0;
            string property = parameters = inputs[propertyIndex].Trim();
            propertyValue = null;
            if (string.IsNullOrEmpty(parameters))
            {
                Print.ParametrizedCommandHint();
                return false;
            }

            bool parseResult;
            int index = Array.FindIndex(properties, 0, properties.Length, i => i.Equals(property, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int valueIndex = 1;
                propertyValue = inputs.Length > 1 ? inputs[valueIndex].Trim().Trim('\'') : null;
                parseResult = true;
            }
            else
            {
                Print.MissedPropertyInfo(property);
                parseResult = false;
            }

            return parseResult;
        }

        /// <summary>
        /// Parses filters to the filters dictionary.
        /// </summary>
        /// <param name="filters">The string representation of the setters.</param>
        /// <param name="filtersDictionary">Parsing destination.</param>
        /// <param name="isAndAlsoCombineMethod">When this method returns, contains true if flters predicates combining method is AndAlso, false if felters predicates combining method is OrElse.</param>
        /// <returns>true if the filters dictionary is filled successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when filters is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when filtersDictionary is null.</exception>
        internal static bool TryParseFilters(string filters, Dictionary<string, List<object>> filtersDictionary, out bool isAndAlsoCombineMethod)
        {
            filters = filters ?? throw new ArgumentNullException(nameof(filters));
            filtersDictionary = filtersDictionary ?? throw new ArgumentNullException(nameof(filtersDictionary));
            filters = filters.Trim();
            isAndAlsoCombineMethod = false;
            if (string.IsNullOrWhiteSpace(filters))
            {
                Print.IncorrectSyntax(filters, WhereLiteral);
                return false;
            }

            if (filters.Contains(AndLiteral, StringComparison.InvariantCultureIgnoreCase))
            {
                isAndAlsoCombineMethod = true;
                return TryParseSetOfFilters(filters, AndLiteral, filtersDictionary);
            }
            else if (filters.Contains(OrLiteral, StringComparison.InvariantCultureIgnoreCase))
            {
                return TryParseSetOfFilters(filters, OrLiteral, filtersDictionary);
            }
            else
            {
                return TryParseOneFilter(filters, WhereLiteral, filtersDictionary);
            }
        }

        private static bool TryParseSetOfSelectors(string selectors, string selectorsSeparator, Dictionary<string, string> destination)
        {
            const string IsPresentLiteral = "+";
            string[] selectedProperties = selectors.Split(selectorsSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            for (int i = 0; i < selectedProperties.Length; i++)
            {
                selectedProperties[i] = selectedProperties[i].Trim();
                if (string.IsNullOrWhiteSpace(selectedProperties[i]))
                {
                    Print.IncorrectSyntax(selectors, selectedProperties[i]);
                    return false;
                }

                parceResult = TryAddToPropertiesNameValuePairs(new string[] { selectedProperties[i], IsPresentLiteral }, selectors, destination);
                if (!parceResult)
                {
                    break;
                }
            }

            return parceResult;
        }

        private static bool TryAddToPropertiesNameValuePairs(string[] splittedPair, string keyValuePair, Dictionary<string, string> destination)
        {
            const int ValidPairLength = 2;
            const int KeyIndex = 0;
            const int ValueIndex = 1;
            if (splittedPair.Length != ValidPairLength)
            {
                Print.IncorrectSyntax(keyValuePair);
                return false;
            }

            if (!destination.ContainsKey(splittedPair[KeyIndex]))
            {
                Print.IncorrectSyntax(splittedPair[KeyIndex]);
                return false;
            }

            if (destination[splittedPair[KeyIndex]] != null)
            {
                Print.RepeatedProperty(splittedPair[KeyIndex]);
                return false;
            }

            destination[splittedPair[KeyIndex]] = splittedPair[ValueIndex];
            return true;
        }

        private static bool TryParseSetOfUpdateSetters(string setters, string propertiesSeparator, Dictionary<string, string> destination)
        {
            string[] filtersKeyValuePairs = setters.Split(propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            foreach (var item in filtersKeyValuePairs)
            {
                parceResult = TryParseOneUpdateSetters(item, destination);
                if (!parceResult)
                {
                    break;
                }
            }

            return parceResult;
        }

        private static bool TryParseOneUpdateSetters(string nameValuePair, Dictionary<string, string> destination)
        {
            const char QuoteLiteral = '\'';
            nameValuePair = nameValuePair.Trim();
            bool result = true;
            string[] splittedPair = null;
            if (string.IsNullOrWhiteSpace(nameValuePair))
            {
                Print.IncorrectSyntax(nameValuePair);
                result = false;
            }
            else
            {
                splittedPair = nameValuePair.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if ((splittedPair.Length & 1) == 1 || splittedPair.Length == 0)
                {
                    Print.IncorrectSyntax(nameValuePair);
                    result = false;
                }
                else
                {
                    for (int i = 0; i < splittedPair.Length; i++)
                    {
                        splittedPair[i] = splittedPair[i].Trim().Trim(QuoteLiteral);
                    }
                }
            }

            return result ? TryAddToPropertiesNameValuePairs(splittedPair, nameValuePair, destination) : result;
        }

        private static bool TryParseOneFilter(string nameValuePair, string propertiesSeparator, Dictionary<string, List<object>> destination)
        {
            nameValuePair = nameValuePair.Trim();
            if (string.IsNullOrWhiteSpace(nameValuePair))
            {
                Print.IncorrectSyntax(nameValuePair, propertiesSeparator);
                return false;
            }

            var splittedPair = nameValuePair.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if ((splittedPair.Length & 1) == 1 || splittedPair.Length == 0)
            {
                Print.IncorrectSyntax(nameValuePair);
                return false;
            }

            for (int i = 0; i < splittedPair.Length; i++)
            {
                splittedPair[i] = splittedPair[i].Trim().Trim('\'');
            }

            return TryAddFilerToDictionary(splittedPair, nameValuePair, destination);
        }

        private static bool TryParseSetOfFilters(string filters, string propertiesSeparator, Dictionary<string, List<object>> destination)
        {
            string[] filtersKeyValuePairs = filters.Split(propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            foreach (var item in filtersKeyValuePairs)
            {
                parceResult = TryParseOneFilter(item, propertiesSeparator, destination);
                if (!parceResult)
                {
                    break;
                }
            }

            return parceResult;
        }

        private static bool TryAddFilerToDictionary(string[] splittedPair, string filterKeyValuePair, Dictionary<string, List<object>> destination)
        {
            const int ValidPairLength = 2;
            const int KeyIndex = 0;
            const int ValueIndex = 1;
            Type recordType = typeof(FileCabinetRecord);
            PropertyInfo[] properties = recordType.GetProperties();
            if (splittedPair.Length != ValidPairLength)
            {
                Print.IncorrectSyntax(filterKeyValuePair);
                return false;
            }

            PropertyInfo property = properties.FirstOrDefault(p => p.Name.Equals(splittedPair[KeyIndex], StringComparison.InvariantCultureIgnoreCase));
            if (property is null)
            {
                Print.IncorrectSyntax(splittedPair[KeyIndex]);
                return false;
            }

            object value = GetValidValue(property.Name, splittedPair[ValueIndex], InputValidator);
            if (value is null)
            {
                return false;
            }

            if (destination.ContainsKey(property.Name))
            {
                destination[property.Name].Add(value);
            }
            else
            {
                var valuesList = new List<object>();
                valuesList.Add(value);
                destination.Add(property.Name, valuesList);
            }

            return true;
        }

        private static object GetValidValue(string propertyName, string value, InputValidator inputValidator)
        {
            switch (propertyName)
            {
                case nameof(FileCabinetRecord.Id):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.IntConverter, inputValidator.ValidateInt, out int id);
                        return isCorrect ? (object)id : null;
                    }

                case nameof(FileCabinetRecord.FirstName):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.StringsConverter, inputValidator.ValidateStrings, out string firstName);
                        return isCorrect ? firstName : null;
                    }

                case nameof(FileCabinetRecord.LastName):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.StringsConverter, inputValidator.ValidateStrings, out string lastName);
                        return isCorrect ? lastName : null;
                    }

                case nameof(FileCabinetRecord.DateOfBirth):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.DatesConverter, inputValidator.ValidateDate, out DateTime date);
                        return isCorrect ? (object)date : null;
                    }

                case nameof(FileCabinetRecord.ZipCode):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.ShortConverter, inputValidator.ValidateShort, out short zipCode);
                        return isCorrect ? (object)zipCode : null;
                    }

                case nameof(FileCabinetRecord.City):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.StringsConverter, inputValidator.ValidateStrings, out string city);
                        return isCorrect ? city : null;
                    }

                case nameof(FileCabinetRecord.Street):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.StringsConverter, inputValidator.ValidateStrings, out string street);
                        return isCorrect ? street : null;
                    }

                case nameof(FileCabinetRecord.Salary):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.DecimalConverter, inputValidator.ValidateDecimal, out decimal salary);
                        return isCorrect ? (object)salary : null;
                    }

                case nameof(FileCabinetRecord.Gender):
                    {
                        bool isCorrect = Input.TryCheckInput(value, Input.CharConverter, inputValidator.ValidateChar, out char gender);
                        return isCorrect ? (object)gender : null;
                    }

                default:
                    {
                        return null;
                    }
            }
        }
    }
}
