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
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreetValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        public StreetValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

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

            if (arguments.Street.Length < this.minLength || arguments.Street.Length > this.maxLength)
            {
                throw new ArgumentException($"{nameof(arguments.Street)}.Length should be from {this.minLength} to {this.maxLength}. {nameof(arguments.Street)} should not consist only of white-spaces characters");
            }
        }
    }
}
