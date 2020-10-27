using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default salary argument and input validator.
    /// </summary>
    internal class SalaryValidator : IRecordValidator<RecordArguments>
    {
        private decimal minValue;
        private decimal maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minValue">Min salary value.</param>
        /// <param name="maxValue">Max salary value.</param>
        public SalaryValidator(decimal minValue, decimal maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Validates Salary argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.Salary < this.minValue || arguments.Salary > this.maxValue)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.Salary)} range is {this.minValue}..{this.maxValue}");
            }
        }
    }
}
