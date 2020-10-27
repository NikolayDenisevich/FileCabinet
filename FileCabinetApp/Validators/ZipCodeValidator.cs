using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default ZipCode argument and input validator.
    /// </summary>
    internal class ZipCodeValidator : IRecordValidator<RecordArguments>
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
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.ZipCode < this.Min || arguments.ZipCode > this.Max)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.ZipCode)} range is {this.Min}..{this.Max}");
            }
        }
    }
}
