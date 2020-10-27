using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides container for FileCabinetRecord validators.
    /// </summary>
    public class ValidationRulesContainer
    {
        /// <summary>
        /// Gets or sets FirstNameValidator instance.
        /// </summary>
        /// <value>
        /// FirstNameValidator instance.
        /// </value>
        public FirstNameValidator FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastNameValidator instance.
        /// </summary>
        /// <value>
        /// LastNameValidator instance.
        /// </value>
        public LastNameValidator LastName { get; set; }

        /// <summary>
        /// Gets or sets DateOfBirthValidator instance.
        /// </summary>
        /// <value>
        /// DateOfBirthValidator instance.
        /// </value>
        public DateOfBirthValidator DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets ZipCodeValidator instance.
        /// </summary>
        /// <value>
        /// ZipCodeValidator instance.
        /// </value>
        public ZipCodeValidator ZipCode { get; set; }

        /// <summary>
        /// Gets or sets CityValidator instance.
        /// </summary>
        /// <value>
        /// CityValidator instance.
        /// </value>
        public CityValidator City { get; set; }

        /// <summary>
        /// Gets or sets StreetValidator instance.
        /// </summary>
        /// <value>
        /// StreetValidator instance.
        /// </value>
        public StreetValidator Street { get; set; }

        /// <summary>
        /// Gets or sets SalaryValidator instance.
        /// </summary>
        /// <value>
        /// SalaryValidator instance.
        /// </value>
        public SalaryValidator Salary { get; set; }

        /// <summary>
        /// Gets or sets GenderValidator instance.
        /// </summary>
        /// <value>
        /// GenderValidator instance.
        /// </value>
        public GenderValidator Gender { get; set; }
    }
}
