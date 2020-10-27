using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides LastName validator.
    /// </summary>
    internal class LastNameValidator : IRecordValidator<RecordArguments>
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
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.LastName is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.LastName)} is null");
            }

            if (arguments.LastName.Length < this.Min || arguments.LastName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.LastName)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.LastName)} should not consist only of white-spaces characters");
            }
        }
    }
}
