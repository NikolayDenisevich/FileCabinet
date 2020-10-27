using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides salary validator.
    /// </summary>
    public class SalaryValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Gets or sets Min salary value.
        /// </summary>
        /// <value>
        /// Min salary value.
        /// </value>
        public decimal Min { get; set; }

        /// <summary>
        /// Gets or sets Max salary value.
        /// </summary>
        /// <value>
        /// Max salary value.
        /// </value>
        public decimal Max { get; set; }

        /// <summary>
        /// Validates Salary argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arguments.Salary is not in the range from Min to Max.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            if (arguments.Salary < this.Min || arguments.Salary > this.Max)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.Salary)} range is {this.Min}..{this.Max}");
            }
        }
    }
}
