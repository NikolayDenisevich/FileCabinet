using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default ZipCode argument and input validator.
    /// </summary>
    public class ZipCodeValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Gets or sets min ZipCode value.
        /// </summary>
        /// <value>
        /// Min ZipCode value.
        /// </value>
        public short Min { get; set; }

        /// <summary>
        /// Gets or sets max ZipCode value.
        /// </summary>
        /// <value>
        /// Max ZipCode value.
        /// </value>
        public short Max { get; set; }

        /// <summary>
        /// Validates ZipCode argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arguments.ZipCode is not in the range from Min to Max.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            if (arguments.ZipCode < this.Min || arguments.ZipCode > this.Max)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.ZipCode)} range is {this.Min}..{this.Max}");
            }
        }
    }
}
