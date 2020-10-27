namespace FileCabinetApp
{
    /// <summary>
    /// Provides arguments record validator.
    /// </summary>
    /// <typeparam name="T">Arguments type.</typeparam>
    public interface IRecordValidator<T>
    {
        /// <summary>
        /// Validates record arguments.
        /// </summary>
        /// <param name="arguments">A set of arguments to validate.</param>
        public void ValidateArguments(T arguments);

        /// <summary>
        /// Returns the InputValidator instance.
        /// </summary>
        /// <returns>The InputValidator instance.</returns>
        public InputValidator GetInputValidator();
    }
}
