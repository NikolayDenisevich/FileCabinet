using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    internal static class Input
    {
        internal static RecordArguments ReadArguments(InputValidator validator)
        {
            Console.Write("First Name: ");
            string firstName = ReadInput(StringsConverter, validator.ValidateStrings);
            Console.Write("Last Name: ");
            string lastName = ReadInput(StringsConverter, validator.ValidateStrings);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(DatesConverter, validator.ValidateDate);
            Console.Write("Zip code: ");
            short zipCode = ReadInput(ShortConverter, validator.ValidateShort);
            Console.Write("City: ");
            string city = ReadInput(StringsConverter, validator.ValidateStrings);
            Console.Write("Street: ");
            string street = ReadInput(StringsConverter, validator.ValidateStrings);
            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, validator.ValidateDecimal);
            Console.Write("Gender: ");
            char gender = ReadInput(CharConverter, validator.ValidateChar);

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

        internal static bool TryToUpdateArgumentsWithId(RecordArguments arguments, Dictionary<string, string> propertiesNameValuePairs, InputValidator validator)
        {
            bool result = true;
            if (propertiesNameValuePairs[nameof(FileCabinetRecord.Id)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.Id)], IntConverter, validator.ValidateInt, out int id);
                arguments.Id = id;
            }

            if (result)
            {
                result &= TryToUpdateArguments(arguments, propertiesNameValuePairs, validator);
            }

            return result;
        }

        internal static bool TryToUpdateArguments(RecordArguments arguments, Dictionary<string, string> propertiesNameValuePairs, InputValidator validator)
        {
            bool result = true;
            if (propertiesNameValuePairs[nameof(FileCabinetRecord.FirstName)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.FirstName)], StringsConverter, validator.ValidateStrings, out string firstName);
                arguments.FirstName = firstName;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.LastName)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.LastName)], StringsConverter, validator.ValidateStrings, out string lastName);
                arguments.LastName = lastName;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.DateOfBirth)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.DateOfBirth)], DatesConverter, validator.ValidateDate, out DateTime dateOfBirth);
                arguments.DateOfBirth = dateOfBirth;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.ZipCode)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.ZipCode)], ShortConverter, validator.ValidateShort, out short zipCode);
                arguments.ZipCode = zipCode;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.City)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.City)], StringsConverter, validator.ValidateStrings, out string city);
                arguments.City = city;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.Street)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.Street)], StringsConverter, validator.ValidateStrings, out string street);
                arguments.Street = street;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.Salary)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.Salary)], DecimalConverter, validator.ValidateDecimal, out decimal salary);
                arguments.Salary = salary;
            }

            if (propertiesNameValuePairs[nameof(FileCabinetRecord.Gender)] != null)
            {
                result &= TryCheckInput(propertiesNameValuePairs[nameof(FileCabinetRecord.Gender)], CharConverter, validator.ValidateChar, out char gender);
                arguments.Gender = gender;
            }

            if (!result)
            {
                Print.ParametrizedCommandHint();
            }

            return result;
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
    }
}
