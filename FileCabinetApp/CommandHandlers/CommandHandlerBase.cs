using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides CommandHandler base class.
    /// </summary>
    internal class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Sets next command handler in the chain.
        /// </summary>
        /// <param name="commandHandler">Next command handler in the chain.</param>
        /// <returns>CommandHandlerBase instance command handler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return this.nextHandler;
        }

        /// <summary>
        /// Handles command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
