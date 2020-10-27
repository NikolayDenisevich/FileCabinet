using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides LastName validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator<RecordArguments>
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
        /// Validates LastName argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.LastName is null.</exception>
        /// <exception cref="ArgumentException">arguments.LastName.Length should be from Min to Max. arguments.LastName should not consist only of white-spaces characters.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            arguments.LastName = arguments.LastName ?? throw new ArgumentNullException($"{nameof(arguments.LastName)}");

            if (arguments.LastName.Length < this.Min || arguments.LastName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.LastName)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.LastName)} should not consist only of white-spaces characters");
            }
        }
    }
}
