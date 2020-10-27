using System;
using System.Collections.Generic;
using System.Globalization;

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
        /// Input validator instance.
        /// </summary>
        internal static InputValidator InputValidator;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Returns a value that indicates whether a record with specified Id is exists in collection.
        /// </summary>
        /// <param name="id">Record Id.</param>
        /// <param name="collection">Records collection.</param>
        /// <returns>true if the record with a specified Id is exists in collection; otherwise - false.</returns>
        internal static bool IsExistsRecordIdInList(int id, IReadOnlyCollection<FileCabinetRecord> collection)
        {
            bool isExists = false;

            foreach (var item in collection)
            {
                if (item.Id == id)
                {
                    isExists = true;
                    break;
                }
            }

            return isExists;
        }

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
        internal static void ParceParameters(Tuple<string, Action<string, string>>[] properties, string parameters)
        {
            var inputs = parameters.Split(' ', 2);
            const int propertyIndex = 0;
            string property = inputs[propertyIndex];

            if (string.IsNullOrEmpty(property))
            {
                PrintParametrizedCommandHint();
                return;
            }

            int index = Array.FindIndex(properties, 0, properties.Length, i => i.Item1.Equals(property, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int valueIndex = 1;
                string propertyValue = inputs.Length > 1 ? inputs[valueIndex] : string.Empty;
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

        private static Tuple<bool, string, string> StringsConverter(string input)
        {
            return new Tuple<bool, string, string>(true, string.Empty, input.Trim());
        }

        private static Tuple<bool, string, DateTime> DatesConverter(string input)
        {
            DateTime dateOfBirth;
            bool isParsed = DateTime.TryParse(input.Trim(), out dateOfBirth);
            string message = "Correct format is: dd/mm/yyyy";
            return new Tuple<bool, string, DateTime>(isParsed, message, dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string input)
        {
            short zipCode;
            bool isParsed = short.TryParse(input.Trim(), out zipCode);
            string message = "Correct format is: 1-4-digit numbers only";
            return new Tuple<bool, string, short>(isParsed, message, zipCode);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            decimal salary;
            bool isParsed = decimal.TryParse(input.Trim(), NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out salary);
            string message = "Correct format is: decimal numbers (for example: 15.65).";
            return new Tuple<bool, string, decimal>(isParsed, message, salary);
        }

        private static Tuple<bool, string, char> CharConverter(string input)
        {
            char gender;
            bool isParsed = char.TryParse(input.Trim(), out gender);
            string message = "Correct format is: one character only";
            return new Tuple<bool, string, char>(isParsed, message, gender);
        }

        private static void PrintMissedPropertyInfo(string propertyName)
        {
            Console.WriteLine($"There is no '{propertyName}' property.");
            Console.WriteLine();
        }
    }
}
