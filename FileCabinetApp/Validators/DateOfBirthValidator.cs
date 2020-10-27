using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default DateOfBirth argument and input validator.
    /// </summary>
    internal class DateOfBirthValidator : IRecordValidator<RecordArguments>
    {
        private DateTime from;
        private DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min date.</param>
        /// <param name="to">Max date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates DateOfBirth argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.DateOfBirth < this.from || arguments.DateOfBirth >= this.to)
            {
                throw new ArgumentException($"{nameof(arguments.DateOfBirth)} should be more than {this.from.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} and not more than {this.to.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} or invalid date format input");
            }
        }
    }
}
