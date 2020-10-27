using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides a chain builder to validate records arguments.
    /// </summary>
    internal class ValidatorBuilder
    {
        private readonly List<IRecordValidator<RecordArguments>> validators = new List<IRecordValidator<RecordArguments>>();

        /// <summary>
        /// Adds FirstNameValidator to a validation list.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator { Min = minLength, Max = maxLength });
            return this;
        }

        /// <summary>
        /// Adds LastNameValidator to a validation list.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator { Min = minLength, Max = maxLength });
            return this;
        }

        /// <summary>
        /// Adds DateOfBirthValidator to a validation list.
        /// </summary>
        /// <param name="from">Min date value.</param>
        /// <param name="to">Max date value.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator { DateFrom = from, DateTo = to });
            return this;
        }

        /// <summary>
        /// Adds ZipCodeValidator to a validation list.
        /// </summary>
        /// <param name="minValue">Min zipCode value.</param>
        /// <param name="maxValue">Max zipCode value.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateZipCode(short minValue, short maxValue)
        {
            this.validators.Add(new ZipCodeValidator { Min = minValue, Max = maxValue });
            return this;
        }

        /// <summary>
        /// Adds CityValidator to a validation list.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateCity(int minLength, int maxLength)
        {
            this.validators.Add(new CityValidator { Min = minLength, Max = maxLength });
            return this;
        }

        /// <summary>
        /// Adds StreetValidator to a validation list.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateStreet(int minLength, int maxLength)
        {
            this.validators.Add(new StreetValidator { Min = minLength, Max = maxLength });
            return this;
        }

        /// <summary>
        /// Adds SalaryValidator to a validation list.
        /// </summary>
        /// <param name="minValue">Min salary value.</param>
        /// <param name="maxValue">Max salary value.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateSalary(decimal minValue, decimal maxValue)
        {
            this.validators.Add(new SalaryValidator { Min = minValue, Max = maxValue });
            return this;
        }

        /// <summary>
        /// Adds GenderValidator to a validation list.
        /// </summary>
        /// <param name="chars">A set of valid chars.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        public ValidatorBuilder ValidateGender(string chars)
        {
            this.validators.Add(new GenderValidator { CharSet = chars });
            return this;
        }

        /// <summary>
        /// Creates Record validator with a set of validators.
        /// </summary>
        /// <returns>A RecordValidator instance.</returns>
        public IRecordValidator<RecordArguments> Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
