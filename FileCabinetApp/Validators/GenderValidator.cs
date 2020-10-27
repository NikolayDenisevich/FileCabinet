using System;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides Gender validator.
    /// </summary>
    internal class GenderValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Gets or sets valid character set.
        /// </summary>
        /// <value>
        /// Valid character set.
        /// </value>
        public char[] CharSet { get; set; } = Array.Empty<char>();

        /// <summary>
        /// Gets or sets string representation of valid character set. Using for correct configuration load during serializing.
        /// </summary>
        /// <value>
        /// String representation of valid character set valid character set.
        /// </value>
        public string Set
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var item in this.CharSet)
                {
                    builder.Append(item);
                }

                return builder.ToString();
            }

            set
            {
                this.CharSet = value.ToCharArray();
            }
        }

        /// <summary>
        /// Validates Gender argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            bool isValid = false;
            foreach (var item in this.CharSet)
            {
                if (arguments.Gender == item)
                {
                    isValid = true;
                    break;
                }
            }

            if (!isValid)
            {
                var builder = new StringBuilder();
                for (int i = 0; i < this.CharSet.Length - 1; i++)
                {
                    builder.Append($"'{this.CharSet[i]}', ");
                }

                builder.Append($"'{this.CharSet[^1]}'.");

                throw new ArgumentException($"{nameof(arguments.Gender)} permissible values are :{builder}");
            }
        }
    }
}
