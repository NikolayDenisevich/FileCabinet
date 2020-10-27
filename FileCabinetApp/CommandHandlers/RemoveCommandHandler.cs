using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'remove' command handler for FileCabinetApp.
    /// </summary>
    internal class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private const string RemoveCommand = "remove";

        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        public RemoveCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'remove' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(RemoveCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Remove(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Remove(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                PrintParametrizedCommandHint();
            }
            else
            {
                int id = this.FindRecordId(parameters);
                if (id > 0)
                {
                    this.service.Remove(id);
                    Console.WriteLine($"Record #{id} is removed.");
                }
            }
        }

        private int FindRecordId(string parameters)
        {
            int id;
            bool isParsed = int.TryParse(parameters, out id);
            if (!isParsed || id == 0)
            {
                Console.WriteLine($"Record #{parameters} doesn't exists.");
                return -1;
            }
            else
            {
                IReadOnlyCollection<FileCabinetRecord> readonlyCollection = this.service.GetRecords();
                bool isExists = IsExistsRecordIdInList(id, readonlyCollection);
                if (!isExists)
                {
                    Console.WriteLine($"Record #{id} doesn't exists.");
                    return -1;
                }
            }

            return id;
        }
    }
}
