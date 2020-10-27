using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default Street argument and input validator.
    /// </summary>
    public class StreetValidator : IRecordValidator<RecordArguments>
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
        /// Validates Street argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.Street is null.</exception>
        /// <exception cref="ArgumentException">arguments.Street.Length should be from Min to Max. arguments.Street should not consist only of white-spaces characters.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            arguments.Street = arguments.Street ?? throw new ArgumentNullException($"{nameof(arguments.Street)}");

            if (arguments.Street.Length < this.Min || arguments.Street.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.Street)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.Street)} should not consist only of white-spaces characters");
            }
        }
    }
}
