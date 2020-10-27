using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default LastName argument and input validator.
    /// </summary>
    internal class LastNameValidator : IRecordValidator<RecordArguments>
    {
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min symbols count.</param>
        /// <param name="maxLength">Max symbols count.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

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

            if (arguments.LastName.Length < this.minLength || arguments.LastName.Length > this.maxLength)
            {
                throw new ArgumentException($"{nameof(arguments.LastName)}.Length should be from {this.minLength} to {this.maxLength}. {nameof(arguments.LastName)} should not consist only of white-spaces characters");
            }
        }
    }
}
