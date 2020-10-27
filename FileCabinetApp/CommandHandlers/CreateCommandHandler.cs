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
        private readonly InputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public CreateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            this.inputValidator = validator;
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
                    Print.UseCommandWithoutParameters(CreateCommand);
                }
                else
                {
                    RecordArguments arguments = Input.ReadArguments(this.inputValidator);
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
