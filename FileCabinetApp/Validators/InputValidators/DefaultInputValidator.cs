using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides the default input validator.
    /// </summary>
    internal sealed class DefaultInputValidator : InputValidator
    {
        /// <summary>
        /// Validates string input.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal override Tuple<bool, string> ValidateStrings(string input)
        {
            bool isValid = !(string.IsNullOrEmpty(input) || input.Length < 2 || input.Length > 60);
            string message = $"Names minimum number of characters is 2, maximum is 60 and cannot be empty or contain only space characters";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates char input.
        /// </summary>
        /// <param name="input">Input character.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal override Tuple<bool, string> ValidateChar(char input)
        {
            bool isValid = !(input != 'm' && input != 'M' && input != 'f' && input != 'F');
            string message = $"Gender permissible values are :'m', 'M', 'f', 'F'.";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates DateTime input.
        /// </summary>
        /// <param name="input">Input DateTime.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal override Tuple<bool, string> ValidateDate(DateTime input)
        {
            var minDate = new DateTime(1950, 1, 1);
            var maxDate = DateTime.Now;
            bool isValid = !(input < minDate || input > maxDate);
            string message = $"Date of birth should be more than {minDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} and not more than {maxDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)}";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates decimal input.
        /// </summary>
        /// <param name="input">Input decimal number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal override Tuple<bool, string> ValidateDecimal(decimal input)
        {
            bool isValid = !(input < 1 || input > 100000);
            string message = $"Salary range is 1..100 000";
            return new Tuple<bool, string>(isValid, message);
        }

        /// <summary>
        /// Validates short input.
        /// </summary>
        /// <param name="input">Input short number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal override Tuple<bool, string> ValidateShort(short input)
        {
            bool isValid = !(input < 1 || input > 9999);
            string message = $"Zip code range is 1..9999";
            return new Tuple<bool, string>(isValid, message);
        }
    }
}
