using System;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides Gender validator.
    /// </summary>
    public class GenderValidator : IRecordValidator<RecordArguments>
    {
        /// <summary>
        /// Gets or sets valid character set.
        /// </summary>
        /// <value>
        /// Valid character set.
        /// </value>
        public string CharSet { get; set; }

        /// <summary>
        /// Validates Gender argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments is null. -or- arguments.City is null.</exception>
        /// <exception cref="ArgumentException">arguments.Gender in not a member of valid characters.</exception>
        public void ValidateArguments(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
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
