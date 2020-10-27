using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides Firstname validator.
    /// </summary>
    internal class FirstNameValidator : IRecordValidator<RecordArguments>
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
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.FirstName is null)
            {
                throw new ArgumentNullException($"{nameof(arguments.FirstName)} is null");
            }

            if (arguments.FirstName.Length < this.Min || arguments.FirstName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(arguments.FirstName)}.Length should be from {this.Min} to {this.Max}. {nameof(arguments.FirstName)} should not consist only of white-spaces characters");
            }
        }
    }
}
