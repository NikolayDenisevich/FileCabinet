namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Defines methods for handle commands.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next command handler in the chain.
        /// </summary>
        /// <param name="commandHandler">Next command handler in the chain.</param>
        /// <returns>CommandHandlerBase instance command handler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles the command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public void Handle(AppCommandRequest commandRequest);
    }
}
