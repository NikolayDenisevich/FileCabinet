using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides Firstname validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator<RecordArguments>
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
        /// Validates FirstName argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.FirstName is null.</exception>
        /// <exception cref="ArgumentException">arguments.CFirstNameity.Length should be from Min to Max. arguments.FirstName should not consist only of white-spaces characters.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            arguments.FirstName = arguments.FirstName ?? throw new ArgumentNullException($"{nameof(arguments.FirstName)}");
            if (arguments.FirstName.Length < this.Min || arguments.FirstName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.FirstName)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.FirstName)} should not consist only of white-spaces characters");
            }
        }
    }
}
