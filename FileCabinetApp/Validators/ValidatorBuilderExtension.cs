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
        /// <param name="container">ValidationRulesContainer instance.</param>
        /// <returns>The default arguments validator instance.</returns>
        internal static IRecordValidator<RecordArguments> CreateValidator(this ValidatorBuilder builder, ValidationRulesContainer container)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(container.FirstNameValidator.Min, container.FirstNameValidator.Max)
                .ValidateLastName(container.LastNameValidator.Min, container.LastNameValidator.Max)
                .ValidateDateOfBirth(container.DateOfBirthValidator.DateFrom, container.DateOfBirthValidator.DateTo)
                .ValidateZipCode(container.ZipCodeValidator.Min, container.ZipCodeValidator.Max)
                .ValidateCity(container.CityValidator.Min, container.CityValidator.Max)
                .ValidateStreet(container.StreetValidator.Min, container.StreetValidator.Max)
                .ValidateSalary(container.SalaryValidator.Min, container.SalaryValidator.Max)
                .ValidateGender(container.GenderValidator.CharSet)
                .Create();
        }
    }
}
