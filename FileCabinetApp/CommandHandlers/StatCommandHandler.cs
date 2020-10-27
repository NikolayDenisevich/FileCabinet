using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'stat' command handler for FileCabinetApp.
    /// </summary>
    internal class StatCommandHandler : ServiceCommandHandlerBase
    {
        private const string StatCommand = "stat";

        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public StatCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Handles the 'stat' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(StatCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                int recordsCount = this.service.GetStat(out int removedRecords);
                Console.WriteLine($"{recordsCount} record(s).");
                Console.WriteLine($"{removedRecords} removed record(s).");
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}
