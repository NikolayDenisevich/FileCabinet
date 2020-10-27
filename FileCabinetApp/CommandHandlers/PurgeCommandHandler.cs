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
        public PurgeCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'purge' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
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
            (int, int) purgeResult = service.Purge();
            if (purgeResult.Item1 == -1)
            {
                Console.WriteLine("There is nothing to purge.");
            }
            else
            {
                Console.WriteLine($"Data file processing is completed: {purgeResult.Item1} of {purgeResult.Item2} records were purged.");
            }
        }
    }
}
