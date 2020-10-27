using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides container for FileCabinetRecord validators.
    /// </summary>
    internal class ValidationRulesContainer
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateOfBirth";
        private const string ZipCode = "zipCode";
        private const string City = "city";
        private const string Street = "street";
        private const string Salary = "salary";
        private const string Gender = "gender";
        private IConfigurationSection configSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRulesContainer"/> class.
        /// </summary>
        /// <param name="configSection">Configuration section.</param>
        public ValidationRulesContainer(IConfigurationSection configSection)
        {
            this.configSection = configSection;
            this.InitializeProperties();
        }

        /// <summary>
        /// Gets FirstNameValidator instance.
        /// </summary>
        /// <value>
        /// FirstNameValidator instance.
        /// </value>
        internal FirstNameValidator FirstNameValidator { get; private set; }

        /// <summary>
        /// Gets LastNameValidator instance.
        /// </summary>
        /// <value>
        /// LastNameValidator instance.
        /// </value>
        internal LastNameValidator LastNameValidator { get; private set; }

        /// <summary>
        /// Gets DateOfBirthValidator instance.
        /// </summary>
        /// <value>
        /// DateOfBirthValidator instance.
        /// </value>
        internal DateOfBirthValidator DateOfBirthValidator { get; private set; }

        /// <summary>
        /// Gets ZipCodeValidator instance.
        /// </summary>
        /// <value>
        /// ZipCodeValidator instance.
        /// </value>
        internal ZipCodeValidator ZipCodeValidator { get; private set; }

        /// <summary>
        /// Gets CityValidator instance.
        /// </summary>
        /// <value>
        /// CityValidator instance.
        /// </value>
        internal CityValidator CityValidator { get; private set; }

        /// <summary>
        /// Gets StreetValidator instance.
        /// </summary>
        /// <value>
        /// StreetValidator instance.
        /// </value>
        internal StreetValidator StreetValidator { get; private set; }

        /// <summary>
        /// Gets SalaryValidator instance.
        /// </summary>
        /// <value>
        /// SalaryValidator instance.
        /// </value>
        internal SalaryValidator SalaryValidator { get; private set; }

        /// <summary>
        /// Gets GenderValidator instance.
        /// </summary>
        /// <value>
        /// GenderValidator instance.
        /// </value>
        internal GenderValidator GenderValidator { get; private set; }

        /// <summary>
        /// Initializes validators properties.
        /// </summary>
        public void InitializeProperties()
        {
            this.FirstNameValidator = this.configSection.GetSection(FirstName).Get<FirstNameValidator>();
            this.LastNameValidator = this.configSection.GetSection(LastName).Get<LastNameValidator>();
            this.DateOfBirthValidator = this.configSection.GetSection(DateOfBirth).Get<DateOfBirthValidator>();
            this.ZipCodeValidator = this.configSection.GetSection(ZipCode).Get<ZipCodeValidator>();
            this.CityValidator = this.configSection.GetSection(City).Get<CityValidator>();
            this.StreetValidator = this.configSection.GetSection(Street).Get<StreetValidator>();
            this.SalaryValidator = this.configSection.GetSection(Salary).Get<SalaryValidator>();
            this.GenderValidator = this.configSection.GetSection(Gender).Get<GenderValidator>();
        }
    }
}
