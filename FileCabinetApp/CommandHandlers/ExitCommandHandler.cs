using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'exit' command handler for FileCabinetApp.
    /// </summary>
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private const string ExitCommand = "exit";
        private Action<bool> actionRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="actionRunning">Sets isRunning field equals false.</param>
        public ExitCommandHandler(Action<bool> actionRunning)
        {
            this.actionRunning = actionRunning;
        }

        /// <summary>
        /// Handles the 'exit' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(ExitCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Exiting an application...");
                this.actionRunning(false);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}
