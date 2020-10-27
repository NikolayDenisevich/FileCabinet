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
        private ValidationRulesContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValidator"/> class.
        /// </summary>
        /// <param name="container">ValidationRulesContainer instance.</param>
        public InputValidator(ValidationRulesContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Validates string input.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateStrings(string input)
        {
            bool isValid = !(string.IsNullOrEmpty(input) || input.Length < this.container.FirstNameValidator.Min || input.Length > this.container.FirstNameValidator.Max);
            string message = $"Names minimum number of characters is {this.container.FirstNameValidator.Min}, maximum is {this.container.FirstNameValidator.Max} and cannot be empty or contain only space characters";
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
            foreach (var item in this.container.GenderValidator.CharSet)
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
                for (int i = 0; i < this.container.GenderValidator.CharSet.Length - 1; i++)
                {
                    builder.Append($"'{this.container.GenderValidator.CharSet[i]}', ");
                }

                builder.Append($"'{this.container.GenderValidator.CharSet[^1]}'.");
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
            var minDate = this.container.DateOfBirthValidator.DateFrom;
            var maxDate = this.container.DateOfBirthValidator.DateTo;
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
            bool isValid = !(input < this.container.SalaryValidator.Min || input > this.container.SalaryValidator.Max);
            string message = $"Salary range is {this.container.SalaryValidator.Min}..{this.container.SalaryValidator.Max}";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates short input.
        /// </summary>
        /// <param name="input">Input short number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal Tuple<bool, string> ValidateShort(short input)
        {
            bool isValid = !(input < this.container.ZipCodeValidator.Min || input > this.container.ZipCodeValidator.Max);
            string message = $"Zip code range is {this.container.ZipCodeValidator.Min}..{this.container.ZipCodeValidator.Max}";
            return new Tuple<bool, string>(isValid, message);
        }
    }
}
