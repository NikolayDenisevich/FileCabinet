using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides composite arguments validator.
    /// </summary>
    internal class CompositeValidator : IRecordValidator<RecordArguments>
    {
        private readonly List<IRecordValidator<RecordArguments>> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when validators is null.</exception>
        public CompositeValidator(IEnumerable<IRecordValidator<RecordArguments>> validators)
        {
            validators = validators ?? throw new ArgumentNullException(nameof(validators));
            this.validators = new List<IRecordValidator<RecordArguments>>(validators);
        }

        /// <summary>
        /// Validates arguments.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(RecordArguments arguments)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateArguments(arguments);
            }
        }
    }
}
