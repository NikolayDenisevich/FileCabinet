﻿using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default Firstname and input validator.
    /// </summary>
    internal class FirstNameValidator : IRecordValidator<RecordArguments>
    {
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Validates FirstName argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.FirstName is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.FirstName)} is null");
            }

            if (arguments.FirstName.Length < this.minLength || arguments.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"{nameof(arguments.FirstName)}.Length should be from {this.minLength} to {this.maxLength}. {nameof(arguments.FirstName)} should not consist only of white-spaces characters");
            }
        }
    }
}
