﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default arguments and input validator.
    /// </summary>
    internal class DefaultValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Returns the DefaultInputValidator instance.
        /// </summary>
        /// <returns>The DefaultInputValidator instance.</returns>
        public InputValidator GetInputValidator()
        {
            return new DefaultInputValidator();
        }

        /// <summary>
        /// Validates arguments.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.FirstName is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.FirstName)} is null");
            }

            if (arguments.FirstName.Length < 2 || arguments.FirstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(arguments.FirstName)}.Length should be from 2 to 60. {nameof(arguments.FirstName)} should not consist only of white-spaces characters");
            }

            if (arguments.LastName is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.LastName)} is null");
            }

            if (arguments.LastName.Length < 2 || arguments.LastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(arguments.LastName)}.Length should be from 2 to 60. {nameof(arguments.LastName)} should not consist only of white-spaces characters");
            }

            if (arguments.DateOfBirth < new DateTime(1950, 1, 1) || arguments.DateOfBirth >= DateTime.Now)
            {
                throw new ArgumentException($"{nameof(arguments.DateOfBirth)} should be more than 01-Jan-1950 and not more than {DateTime.Now.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} or invalid date format input");
            }

            if (arguments.ZipCode < 1 || arguments.ZipCode > 9999)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.ZipCode)} range is 1..9999");
            }

            if (arguments.City is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.City)} is null");
            }

            if (arguments.City.Length < 2 || arguments.City.Length > 60)
            {
                throw new ArgumentException($"{nameof(arguments.City)}.Length should be from 2 to 60. {nameof(arguments.City)} should not consist only of white-spaces characters");
            }

            if (arguments.Street is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.Street)} is null");
            }

            if (arguments.Street.Length < 2 || arguments.Street.Length > 60)
            {
                throw new ArgumentException($"{nameof(arguments.Street)}.Length should be from 2 to 60. {nameof(arguments.Street)} should not consist only of white-spaces characters");
            }

            if (arguments.Salary < 0 || arguments.Salary > 100000)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.Salary)} range is 0..100 000");
            }

            if (arguments.Gender != 'm' && arguments.Gender != 'M' && arguments.Gender != 'f' && arguments.Gender != 'F')
            {
                throw new ArgumentException($"{nameof(arguments.Gender)} permissible values are :'m', 'M', 'f', 'F'.");
            }
        }
    }
}
