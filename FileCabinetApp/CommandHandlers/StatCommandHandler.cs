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
        public StatCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'stat' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(StatCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                var recordsCount = this.service.GetStat();
                Console.WriteLine($"{recordsCount.Item1} record(s).");
                Console.WriteLine($"{recordsCount.Item2} removed record(s).");
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}
