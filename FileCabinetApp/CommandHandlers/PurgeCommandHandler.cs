using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'purge' command handler for FileCabinetApp.
    /// </summary>
    internal class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string PurgeCommand = "purge";

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public PurgeCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Handles the 'purge' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(PurgeCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Purge();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Purge()
        {
            int purgedRecords = service.Purge(out int totalRecords);
            if (purgedRecords == -1)
            {
                Console.WriteLine("There is nothing to purge.");
            }
            else
            {
                Console.WriteLine($"Data file processing is completed: {purgedRecords} of {totalRecords} records were purged.");
            }
        }
    }
}
