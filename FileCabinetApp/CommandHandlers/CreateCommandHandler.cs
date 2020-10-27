using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'create' command handler for FileCabinetApp.
    /// </summary>
    internal class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CreateCommand = "create";
        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public CreateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            InputValidator = validator;
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'create' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(CreateCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(commandRequest.Parameters))
                {
                    Console.WriteLine("Please, use 'create' command without any parameters.");
                }
                else
                {
                    RecordArguments arguments = ReadArguments();
                    int id = this.service.CreateRecord(arguments);
                    Console.WriteLine($"Record #{id} is created");
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}
