using System;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides default Gender argument and input validator.
    /// </summary>
    internal class GenderValidator : IRecordValidator<RecordArguments>
    {
        private char[] chars;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="chars">A set of valid chars.</param>
        public GenderValidator(params char[] chars)
        {
            this.chars = chars;
        }

        /// <summary>
        /// Validates Gender argument.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            bool isValid = false;
            foreach (var item in this.chars)
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
                for (int i = 0; i < this.chars.Length - 1; i++)
                {
                    builder.Append($"'{this.chars[i]}', ");
                }

                builder.Append($"'{this.chars[^1]}'.");

                throw new ArgumentException($"{nameof(arguments.Gender)} permissible values are :{builder.ToString()}");
            }
        }
    }
}
