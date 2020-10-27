using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default ZipCode argument and input validator.
    /// </summary>
    internal class ZipCodeValidator : IRecordValidator<RecordArguments>
    {
        private short minValue;
        private short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipCodeValidator"/> class.
        /// </summary>
        /// <param name="minValue">Min zipCode value.</param>
        /// <param name="maxValue">Max zipCode value.</param>
        public ZipCodeValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Validates ZipCode argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.ZipCode < this.minValue || arguments.ZipCode > this.maxValue)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.ZipCode)} range is {this.minValue}..{this.maxValue}");
            }
        }
    }
}
