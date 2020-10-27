using System;

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
        /// <exception cref="ArgumentNullException">Thrown when builder is null. -or- container is null.</exception>
        internal static IRecordValidator<RecordArguments> CreateValidator(this ValidatorBuilder builder, ValidationRulesContainer container)
        {
            builder = builder ?? throw new ArgumentNullException($"{nameof(builder)}");
            container = container ?? throw new ArgumentNullException($"{nameof(container)}");
            return builder
                .ValidateFirstName(container.FirstName.Min, container.FirstName.Max)
                .ValidateLastName(container.LastName.Min, container.LastName.Max)
                .ValidateDateOfBirth(container.DateOfBirth.DateFrom, container.DateOfBirth.DateTo)
                .ValidateZipCode(container.ZipCode.Min, container.ZipCode.Max)
                .ValidateCity(container.City.Min, container.City.Max)
                .ValidateStreet(container.Street.Min, container.Street.Max)
                .ValidateSalary(container.Salary.Min, container.Salary.Max)
                .ValidateGender(container.Gender.CharSet)
                .Create();
        }
    }
}
