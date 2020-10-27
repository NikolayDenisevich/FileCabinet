using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>
        /// Returns default arguments validator.
        /// </summary>
        /// <param name="builder">ValidetorBuilder instance.</param>
        /// <returns>The default arguments validator instance.</returns>
        internal static IRecordValidator<RecordArguments> CreateDefaultValidator(this ValidatorBuilder builder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateZipCode(1, 9999)
                .ValidateCity(2, 60)
                .ValidateStreet(2, 60)
                .ValidateSalary(0, 100000)
                .ValidateGender('f', 'F', 'm', 'M')
                .Create();
        }

        /// <summary>
        /// Returns custom arguments validator.
        /// </summary>
        /// <param name="builder">ValidetorBuilder instance.</param>
        /// <returns>The custom arguments validator instance.</returns>
        internal static IRecordValidator<RecordArguments> CreateCustomValidator(this ValidatorBuilder builder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(3, 30)
                .ValidateLastName(3, 30)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), new DateTime(2002, 1, 1))
                .ValidateZipCode(1000, 9999)
                .ValidateCity(3, 30)
                .ValidateStreet(3, 30)
                .ValidateSalary(100, 5000)
                .ValidateGender('f', 'F', 'm', 'M')
                .Create();
        }
    }
}
