using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides the input validator.
    /// </summary>
    internal sealed class InputValidator
    {
        private readonly ValidationRulesContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValidator"/> class.
        /// </summary>
        /// <param name="container">ValidationRulesContainer instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when container is null.</exception>
        public InputValidator(ValidationRulesContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Validates string input.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateStrings(string input)
        {
            bool isValid = !(string.IsNullOrEmpty(input) || input.Length < this.container.FirstName.Min || input.Length > this.container.FirstName.Max);
            string message = $"Names minimum number of characters is {this.container.FirstName.Min}, maximum is {this.container.FirstName.Max} and cannot be empty or contain only space characters";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates char input.
        /// </summary>
        /// <param name="input">Input character.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateChar(char input)
        {
            bool isValid = false;
            foreach (var item in this.container.Gender.CharSet)
            {
                if (input == item)
                {
                    isValid = true;
                    break;
                }
            }

            var builder = new StringBuilder();
            if (!isValid)
            {
                for (int i = 0; i < this.container.Gender.CharSet.Length - 1; i++)
                {
                    builder.Append($"'{this.container.Gender.CharSet[i]}', ");
                }

                builder.Append($"'{this.container.Gender.CharSet[^1]}'.");
            }

            string message = $"Gender permissible values are : {builder}.";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates DateTime input.
        /// </summary>
        /// <param name="input">Input DateTime.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateDate(DateTime input)
        {
            var minDate = this.container.DateOfBirth.DateFrom;
            var maxDate = this.container.DateOfBirth.DateTo;
            bool isValid = !(input < minDate || input > maxDate);
            string message = $"Date of birth should be more than {minDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} and not more than {maxDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)}";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates decimal input.
        /// </summary>
        /// <param name="input">Input decimal number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateDecimal(decimal input)
        {
            bool isValid = !(input < this.container.Salary.Min || input > this.container.Salary.Max);
            string message = $"Salary range is {this.container.Salary.Min}..{this.container.Salary.Max}";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates short input.
        /// </summary>
        /// <param name="input">Input short number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateShort(short input)
        {
            bool isValid = !(input < this.container.ZipCode.Min || input > this.container.ZipCode.Max);
            string message = $"Zip code range is {this.container.ZipCode.Min}..{this.container.ZipCode.Max}";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates int input.
        /// </summary>
        /// <param name="input">Input int number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateInt(int input)
        {
            bool isValid = input > 0;
            string message = $"ID should be more than zero";
            return new Tuple<bool, string>(isValid, message);
        }
    }
}
