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
        /// <param name="inputValidator">InputValidator instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when inputValidator is null.</exception>
        public CreateCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator inputValidator)
        {
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            this.inputValidator = inputValidator ?? throw new ArgumentNullException(nameof(inputValidator));
        }

        /// <summary>
        /// Handles the 'create' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
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
