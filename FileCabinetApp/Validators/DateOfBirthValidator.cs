using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides DateOfBirth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator<RecordArguments>
    {
        private const string DatePattern = "dd/MM/yyyy";

        /// <summary>
        /// Gets or sets minimum date.
        /// </summary>
        /// <value>
        /// Minimum date.
        /// </value>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Gets or sets maximum date.
        /// </summary>
        /// <value>
        /// Maximum date.
        /// </value>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Gets or sets string representation of minimum date (dd/MM/yyyy format). Using for correct configuration load during serializing.
        /// </summary>
        /// <value>
        /// String representation of minimum date (dd/MM/yyyy format).
        /// </value>
        public string From
        {
            get => this.DateFrom.ToString(DatePattern, DateTimeFormatInfo.InvariantInfo);

            set
            {
                DateTime result;
                _ = DateTime.TryParse(value, out result);
                this.DateFrom = result;
            }
        }

        /// <summary>
        /// Gets or sets string representation of maximum date (dd/MM/yyyy format). Using for correct configuration load during serializing.
        /// </summary>
        /// <value>
        /// String representation of maximum date (dd/MM/yyyy format).
        /// </value>
        public string To
        {
            get => this.DateTo.ToString(DatePattern, DateTimeFormatInfo.InvariantInfo);

            set
            {
                DateTime result;
                _ = DateTime.TryParse(value, out result);
                this.DateTo = result;
            }
        }

        /// <summary>
        /// Validates DateOfBirth argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.City is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arguments.DateOfBirth should be more than DateFrom and not more than DateTo or invalid date format input.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            if (arguments.DateOfBirth < this.DateFrom || arguments.DateOfBirth >= this.DateTo)
            {
                throw new ArgumentOutOfRangeException($"{nameof(arguments.DateOfBirth)} should be more than {this.DateFrom.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} and not more than {this.DateTo.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} or invalid date format input");
            }
        }
    }
}
