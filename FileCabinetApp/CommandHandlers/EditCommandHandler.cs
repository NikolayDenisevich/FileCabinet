using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'edit' command handler for FileCabinetApp.
    /// </summary>
    internal class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string EditCommand = "edit";
        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public EditCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            InputValidator = validator;
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'edit' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(EditCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Edit(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Edit(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isParsed = int.TryParse(parameters, out int id);
            if (!isParsed || id == 0)
            {
                Console.WriteLine($"Record {parameters} is not found.");
                return;
            }

            IReadOnlyCollection<FileCabinetRecord> readonlyCollection = this.service.GetRecords();
            bool isExists = IsExistsRecordIdInList(id, readonlyCollection);
            if (!isExists)
            {
                Console.WriteLine($"Record #{id} is not found.");
                return;
            }

            RecordArguments arguments = ReadArguments();
            arguments.Id = id;

            this.service.EditRecord(arguments);
            Console.WriteLine($"Record #{id} is updated");
        }
    }
}
