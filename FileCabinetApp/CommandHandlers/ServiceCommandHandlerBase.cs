using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides CommandHandler base class.
    /// </summary>
    internal class ServiceCommandHandlerBase : CommandHandlerBase
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// Hint message.
        /// </summary>
        internal const string HintMessage = "You should use this command with valid parameters and values. Enter 'help <commandname>' to get help.";

        /// <summary>
        /// Name of 'Id' FileCabinetRecord property.
        /// </summary>
        internal const string IdName = "id";

        /// <summary>
        /// Name of 'FirstName' FileCabinetRecord property.
        /// </summary>
        internal const string FirstNameName = "firstname";

        /// <summary>
        /// Name of 'LastName' FileCabinetRecord property.
        /// </summary>
        internal const string LastNameName = "lastname";

        /// <summary>
        /// Name of 'DateOfBirth' FileCabinetRecord property.
        /// </summary>
        internal const string DateOfBirthName = "dateofbirth";

        /// <summary>
        /// Name of 'ZipCode' FileCabinetRecord property.
        /// </summary>
        internal const string ZipCodeName = "zipcode";

        /// <summary>
        /// Name of 'City' FileCabinetRecord property.
        /// </summary>
        internal const string CityName = "city";

        /// <summary>
        /// Name of 'Street' FileCabinetRecord property.
        /// </summary>
        internal const string StreetName = "street";

        /// <summary>
        /// Name of 'Salary' FileCabinetRecord property.
        /// </summary>
        internal const string SalaryName = "salary";

        /// <summary>
        /// Name of 'Salary' FileCabinetRecord property.
        /// </summary>
        internal const string GenderName = "gender";

        /// <summary>
        /// Input validator instance.
        /// </summary>
        internal static InputValidator InputValidator;

        /// <summary>
        /// Container for FileCabinetRecord properies names and values string representations for parsing.
        /// </summary>
        internal static Dictionary<string, string> PropertiesNameValuePairs;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        public ServiceCommandHandlerBase()
        {
            if (PropertiesNameValuePairs is null)
            {
                this.InitializePropertiesNameValuePairs();
            }
        }

        // TODO: DEL!
        // /// <summary>
        // /// Returns a value that indicates whether a record with specified Id is exists in collection.
        // /// </summary>
        // /// <param name="id">Record Id.</param>
        // /// <param name="collection">Records collection.</param>
        // /// <returns>true if the record with a specified Id is exists in collection; otherwise - false.</returns>
        // internal static bool IsExistsRecordIdInList(int id, IReadOnlyCollection<FileCabinetRecord> collection)
        // {
        //     bool isExists = false;
        //     foreach (var item in collection)
        //     {
        //         if (item.Id == id)
        //         {
        //             isExists = true;
        //             break;
        //         }
        //     }
        //     return isExists;
        // }

        /// <summary>
        /// Reads FileCabinetRecord arguments using InputValidator instance.
        /// </summary>
        /// <returns>Record arguments instance.</returns>
        internal static RecordArguments ReadArguments()
        {
            Console.Write("First Name: ");
            string firstName = ReadInput(StringsConverter, InputValidator.ValidateStrings);
            Console.Write("Last Name: ");
            string lastName = ReadInput(StringsConverter, InputValidator.ValidateStrings);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(DatesConverter, InputValidator.ValidateDate);
            Console.Write("Zip code: ");
            short zipCode = ReadInput(ShortConverter, InputValidator.ValidateShort);
            Console.Write("City: ");
            string city = ReadInput(StringsConverter, InputValidator.ValidateStrings);
            Console.Write("Street: ");
            string street = ReadInput(StringsConverter, InputValidator.ValidateStrings);
            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, InputValidator.ValidateDecimal);
            Console.Write("Gender: ");
            char gender = ReadInput(CharConverter, InputValidator.ValidateChar);

            var arguments = new RecordArguments
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ZipCode = zipCode,
                City = city,
                Street = street,
                Salary = salary,
                Gender = gender,
            };
            return arguments;
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
                PrintParametrizedCommandHint();
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
                PrintMissedPropertyInfo(property);
            }
        }

        /// <summary>
        /// Prints hint about parametrized commnds using.
        /// </summary>
        internal static void PrintParametrizedCommandHint()
        {
            Console.WriteLine(HintMessage);
        }

        /// <summary>
        /// Prints incorrect syntax mesage.
        /// </summary>
        /// <param name="input">Incorrect data string.</param>
        internal static void PrintIncorrectSyntax(string input)
        {
            Console.WriteLine($"Incorrect syntax: {input}");
            PrintParametrizedCommandHint();
        }

        /// <summary>
        /// Prints incorrect syntax mesage.
        /// </summary>
        /// <param name="input">Incorrect data.</param>
        /// <param name="parameter">Incorrect parameter.</param>
        internal static void PrintIncorrectSyntax(string input, string parameter)
        {
            Console.WriteLine($"Incorrect syntax: {input} : {parameter}");
            PrintParametrizedCommandHint();
        }

        /// <summary>
        /// Converts string input to Int32.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is Int32 convertion result.</returns>
        internal static Tuple<bool, string, int> IntConverter(string input)
        {
            bool isParsed = int.TryParse(input.Trim(), out int id);
            string message = "Correct format is: integer numbers (for example: 15).";
            return new Tuple<bool, string, int>(isParsed, message, id);
        }

        /// <summary>
        /// Converts string input to string.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is string convertion result.</returns>
        internal static Tuple<bool, string, string> StringsConverter(string input)
        {
            return new Tuple<bool, string, string>(true, string.Empty, input.Trim());
        }

        /// <summary>
        /// Converts string input to DateTime.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is DateTime convertion result.</returns>
        internal static Tuple<bool, string, DateTime> DatesConverter(string input)
        {
            bool isParsed = DateTime.TryParse(input.Trim(), out DateTime dateOfBirth);
            string message = "Correct format is: dd/mm/yyyy";
            return new Tuple<bool, string, DateTime>(isParsed, message, dateOfBirth);
        }

        /// <summary>
        /// Converts string input to Int16.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is Int16 convertion result.</returns>
        internal static Tuple<bool, string, short> ShortConverter(string input)
        {
            bool isParsed = short.TryParse(input.Trim(), out short zipCode);
            string message = "Correct format is: 1-4-digit numbers only";
            return new Tuple<bool, string, short>(isParsed, message, zipCode);
        }

        /// <summary>
        /// Converts string input to Decimal.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is Decimal convertion result.</returns>
        internal static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            decimal salary;
            bool isParsed = decimal.TryParse(input.Trim(), NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out salary);
            string message = "Correct format is: decimal numbers (for example: 15.65).";
            return new Tuple<bool, string, decimal>(isParsed, message, salary);
        }

        /// <summary>
        /// Converts string input to Char.
        /// </summary>
        /// <param name="input">string input.</param>
        /// <returns>Item1 is indicates whether the conversion succeeded. Item2 is conversion failed expalnation. Item3 is Char convertion result.</returns>
        internal static Tuple<bool, string, char> CharConverter(string input)
        {
            bool isParsed = char.TryParse(input.Trim(), out char gender);
            string message = "Correct format is: one character only";
            return new Tuple<bool, string, char>(isParsed, message, gender);
        }

        /// <summary>
        /// Sets all propertiesNamesValuePairs dictionary values to null.
        /// </summary>
        /// <param name="propertiesNamesValuePairs">Name-Value pairs to set to null.</param>
        internal static void BreakLastKeyValuePairsState(Dictionary<string, string> propertiesNamesValuePairs)
        {
            propertiesNamesValuePairs[IdName] = null;
            propertiesNamesValuePairs[FirstNameName] = null;
            propertiesNamesValuePairs[LastNameName] = null;
            propertiesNamesValuePairs[DateOfBirthName] = null;
            propertiesNamesValuePairs[ZipCodeName] = null;
            propertiesNamesValuePairs[CityName] = null;
            propertiesNamesValuePairs[StreetName] = null;
            propertiesNamesValuePairs[SalaryName] = null;
            propertiesNamesValuePairs[GenderName] = null;
        }

        /// <summary>
        /// Checks input parameter for valid data type and valid data values.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="input">A string containing a value to convert.</param>
        /// <param name="converter">Converter from string to T type.</param>
        /// <param name="validator">Input data validator.</param>
        /// <param name="value">When this method returns, contains the value equivalent to the type of T contained in input, if the conversion succeeded, or default value if the conversion failed. The conversion fails if the input parameter is null, is an empty string (""), or does not contain a valid string representation of a T value. This parameter is passed uninitialized.</param>
        /// <returns>true if the input parameter was converted successfully; otherwise, false.</returns>
        internal static bool TryCheckInput<T>(string input, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator, out T value)
        {
            var conversionResult = converter(input);
            if (!conversionResult.Item1)
            {
                Console.WriteLine($"Conversion failed: parameter '{input}' - {conversionResult.Item2}. Please, correct your input.");
                value = default;
                return false;
            }

            value = conversionResult.Item3;
            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                Console.WriteLine($"Validation failed: parameter '{input}' - {validationResult.Item2}. Please, correct your input.");
                return false;
            }

            return true;
        }

        internal static void PrintOperationResult(IEnumerable<FileCabinetRecord> operatedRecords, string operationName)
        {
            const int ExtraSymbolsToRemoveBuidlerOffsetFromEnd = 2;
            const int ExtraSymbolsBuidlerCount = 2;
            var enumerator = operatedRecords.GetEnumerator();
            enumerator.MoveNext();
            var record = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                PrintOperationResult(record.Id, operationName);
                return;
            }

            var builder = new StringBuilder();
            builder.Append("Records ");
            foreach (var item in operatedRecords)
            {
                builder.Append($"#{item.Id}, ");
            }

            builder.Remove(builder.Length - ExtraSymbolsToRemoveBuidlerOffsetFromEnd, ExtraSymbolsBuidlerCount);
            builder.Append($" are {operationName}d.");
            Console.WriteLine(builder);
        }

        internal static bool TryToUpdateArguments(RecordArguments arguments, Dictionary<string, string> propertiesNameValuePairs)
        {
            bool result = true;
            if (propertiesNameValuePairs[FirstNameName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[FirstNameName], StringsConverter, InputValidator.ValidateStrings, out string firstName);
                arguments.FirstName = firstName;
            }

            if (propertiesNameValuePairs[LastNameName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[LastNameName], StringsConverter, InputValidator.ValidateStrings, out string lastName);
                arguments.LastName = lastName;
            }

            if (propertiesNameValuePairs[DateOfBirthName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[DateOfBirthName], DatesConverter, InputValidator.ValidateDate, out DateTime dateOfBirth);
                arguments.DateOfBirth = dateOfBirth;
            }

            if (propertiesNameValuePairs[ZipCodeName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[ZipCodeName], ShortConverter, InputValidator.ValidateShort, out short zipCode);
                arguments.ZipCode = zipCode;
            }

            if (propertiesNameValuePairs[CityName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[CityName], StringsConverter, InputValidator.ValidateStrings, out string city);
                arguments.City = city;
            }

            if (propertiesNameValuePairs[StreetName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[StreetName], StringsConverter, InputValidator.ValidateStrings, out string street);
                arguments.Street = street;
            }

            if (propertiesNameValuePairs[SalaryName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[SalaryName], DecimalConverter, InputValidator.ValidateDecimal, out decimal salary);
                arguments.Salary = salary;
            }

            if (propertiesNameValuePairs[GenderName] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[GenderName], CharConverter, InputValidator.ValidateChar, out char gender);
                arguments.Gender = gender;
            }

            if (!result)
            {
                PrintParametrizedCommandHint();
            }

            return result;
        }

        private static void PrintOperationResult(int id, string operationName)
        {
            Console.WriteLine($"Record #{id} is {operationName}d.");
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static void PrintMissedPropertyInfo(string propertyName)
        {
            Console.WriteLine($"There is no '{propertyName}' property.");
            Console.WriteLine();
        }

        private void InitializePropertiesNameValuePairs()
        {
            PropertiesNameValuePairs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            PropertiesNameValuePairs.Add(IdName, null);
            PropertiesNameValuePairs.Add(FirstNameName, null);
            PropertiesNameValuePairs.Add(LastNameName, null);
            PropertiesNameValuePairs.Add(DateOfBirthName, null);
            PropertiesNameValuePairs.Add(ZipCodeName, null);
            PropertiesNameValuePairs.Add(CityName, null);
            PropertiesNameValuePairs.Add(StreetName, null);
            PropertiesNameValuePairs.Add(SalaryName, null);
            PropertiesNameValuePairs.Add(GenderName, null);
        }
    }
}
