using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides DateOfBirth validator.
    /// </summary>
    internal class DateOfBirthValidator : IRecordValidator<RecordArguments>
    {
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
            get => this.DateFrom.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo);

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
            get => this.DateTo.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo);

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
        public void ValidateArguments(RecordArguments arguments)
        {
            if (arguments.DateOfBirth < this.DateFrom || arguments.DateOfBirth >= this.DateTo)
            {
                throw new ArgumentException($"{nameof(arguments.DateOfBirth)} should be more than {this.DateFrom.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} and not more than {this.DateTo.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} or invalid date format input");
            }
        }
    }
}
