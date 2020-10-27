using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'exit' command handler for FileCabinetApp.
    /// </summary>
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private const string ExitCommand = "exit";
        private readonly Action<bool> actionRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="actionRunning">Sets filecabinetservice 'isRunning' flag equals to false.</param>
        /// <exception cref="ArgumentNullException">Thrown when actionRunning is null.</exception>
        public ExitCommandHandler(Action<bool> actionRunning)
        {
            this.actionRunning = actionRunning ?? throw new ArgumentNullException(nameof(actionRunning));
        }

        /// <summary>
        /// Handles the 'exit' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
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
