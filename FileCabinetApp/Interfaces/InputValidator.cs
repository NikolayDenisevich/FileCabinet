using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides input validator.
    /// </summary>
    public abstract class InputValidator
    {
        /// <summary>
        /// Validates string input.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal abstract Tuple<bool, string> ValidateStrings(string input);

        /// <summary>
        /// Validates DateTime input.
        /// </summary>
        /// <param name="input">Input DateTime.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal abstract Tuple<bool, string> ValidateDate(DateTime input);

        /// <summary>
        /// Validates char input.
        /// </summary>
        /// <param name="input">Input character.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal abstract Tuple<bool, string> ValidateChar(char input);

        /// <summary>
        /// Validates decimal input.
        /// </summary>
        /// <param name="input">Input decimal number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal abstract Tuple<bool, string> ValidateDecimal(decimal input);

        /// <summary>
        /// Validates short input.
        /// </summary>
        /// <param name="input">Input short number.</param>
        /// <returns>Tuple(bool, string). bool value indicates validation result. string value representes a message with valid input parameters.</returns>
        internal abstract Tuple<bool, string> ValidateShort(short input);
    }
}
