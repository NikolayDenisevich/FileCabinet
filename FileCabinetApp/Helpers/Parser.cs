using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    internal static class Parser
    {
        private const string WhereLiteral = "where ";
        private const string AndLiteral = " and ";
        private const string OrLiteral = " or ";

        internal static InputValidator InputValidator { get; set; }

        internal static bool TryParceSelectors(string selectors, Dictionary<string, string> selectorsDictionary)
        {
            const string CommaPropertiesSeparator = ",";
            SetValueToAllDictionaryEntries<FileCabinetRecord>(selectorsDictionary, null);
            selectors = selectors.Trim();
            if (string.IsNullOrWhiteSpace(selectors))
            {
                Print.IncorrectSyntax(selectors);
                return false;
            }

            return TryParceSetOfSelectors(selectors, CommaPropertiesSeparator, selectorsDictionary);
        }

        internal static bool TryParceUpdateSetters(string setters, Dictionary<string, string> settersDictionary)
        {
            const string CommaPropertiesSeparator = ",";
            Parser.SetValueToAllDictionaryEntries<FileCabinetRecord>(settersDictionary, null);
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

            return TryParceSetOfUpdateSetters(setters, CommaPropertiesSeparator, settersDictionary);
        }

        /// <summary>
        /// Sets all propertiesNamesValuePairs dictionary values to null.
        /// </summary>
        /// <param name="propertiesNamesValuePairs">Name-Value pairs to set to null.</param>
        /// <param name="value">Value to set to all dictionary entries.</param>
        /// <typeparam name="TRecord">Record type.</typeparam>
        internal static void SetValueToAllDictionaryEntries<TRecord>(Dictionary<string, string> propertiesNamesValuePairs, string value)
        {
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
        /// <param name="charSeparator">Сharacter separating the parameter and its value.</param>
        internal static void ParceParameters(Tuple<string, Action<string, string>>[] properties, string parameters, char charSeparator)
        {
            var inputs = parameters.Split(charSeparator, 2);
            const int propertyIndex = 0;
            string property = inputs[propertyIndex].Trim();

            if (string.IsNullOrEmpty(property))
            {
                Print.ParametrizedCommandHint();
                return;
            }

            int index = Array.FindIndex(properties, 0, properties.Length, i => i.Item1.Equals(property, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int valueIndex = 1;
                string propertyValue = inputs.Length > 1 ? inputs[valueIndex].Trim().Trim('\'') : string.Empty;
                properties[index].Item2(properties[index].Item1, propertyValue);
            }
            else
            {
                Print.MissedPropertyInfo(property);
            }
        }

        internal static bool TryParceFilters(string filters, Dictionary<string, List<object>> filtersDictionary, out bool isAndAlsoCombineMethod)
        {
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
                return TryParceSetOfFilters(filters, AndLiteral, filtersDictionary);
            }
            else if (filters.Contains(OrLiteral, StringComparison.InvariantCultureIgnoreCase))
            {
                return TryParceSetOfFilters(filters, OrLiteral, filtersDictionary);
            }
            else
            {
                return TryParceOneFilter(filters, WhereLiteral, filtersDictionary);
            }
        }

        private static bool TryParceSetOfSelectors(string selectors, string selectorsSeparator, Dictionary<string, string> destination)
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

        private static bool TryAddToPropertiesNameValuePairs(string[] splittedPair, string filterKeyValuePair, Dictionary<string, string> destination)
        {
            const int ValidPairLength = 2;
            const int KeyIndex = 0;
            const int ValueIndex = 1;
            if (splittedPair.Length != ValidPairLength)
            {
                Print.IncorrectSyntax(filterKeyValuePair);
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

        private static bool TryParceSetOfUpdateSetters(string setters, string propertiesSeparator, Dictionary<string, string> destination)
        {
            string[] filtersKeyValuePairs = setters.Split(propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            foreach (var item in filtersKeyValuePairs)
            {
                parceResult = TryParceOneUpdateSetters(item, destination);
                if (!parceResult)
                {
                    break;
                }
            }

            return parceResult;
        }

        private static bool TryParceOneUpdateSetters(string nameValuePair, Dictionary<string, string> destination)
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

        private static bool TryParceOneFilter(string nameValuePair, string propertiesSeparator, Dictionary<string, List<object>> destination)
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

        private static bool TryParceSetOfFilters(string filters, string propertiesSeparator, Dictionary<string, List<object>> destination)
        {
            string[] filtersKeyValuePairs = filters.Split(propertiesSeparator, StringSplitOptions.RemoveEmptyEntries);
            bool parceResult = false;
            foreach (var item in filtersKeyValuePairs)
            {
                parceResult = TryParceOneFilter(item, propertiesSeparator, destination);
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
