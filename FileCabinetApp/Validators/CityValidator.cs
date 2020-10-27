using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides City validator.
    /// </summary>
    public class CityValidator : IRecordValidator<RecordArguments>
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
        /// Validates City argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.City is null.</exception>
        /// <exception cref="ArgumentException">arguments.City.Length should be from Min to Max. arguments.City should not consist only of white-spaces characters.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            arguments.City = arguments.City ?? throw new ArgumentException($"{nameof(arguments.City)}");

            if (arguments.City.Length < this.Min || arguments.City.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.City)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.City)} should not consist only of white-spaces characters");
            }
        }
    }
}
