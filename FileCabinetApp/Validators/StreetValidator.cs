using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default Street argument and input validator.
    /// </summary>
    internal class StreetValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Gets or sets minimum amount of characters.
        /// </summary>
        /// <value>
        /// Minimum amount of characters.
        /// </value>
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets maximum amount of characters.
        /// </summary>
        /// <value>
        /// Maximum amount of characters.
        /// </value>
        public int Max { get; set; }

        /// <summary>
        /// Validates Street argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.Street is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.Street)} is null");
            }

            if (arguments.Street.Length < this.Min || arguments.Street.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.Street)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.Street)} should not consist only of white-spaces characters");
            }
        }
    }
}
