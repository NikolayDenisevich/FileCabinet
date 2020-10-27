using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'delete' command handler for FileCabinetApp.
    /// </summary>
    internal class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string DeleteCommand = "delete";
        private const string WhereLiteral = "where ";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public DeleteCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Handles the 'delete' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(DeleteCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Delete(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Delete(string parameters)
        {
            bool result = IsCorrectWhereParameter(parameters);
            if (!result)
            {
                return;
            }

            string filters = parameters.Replace(WhereLiteral, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
            result = TryGetFilteredCollection(filters, service, out IEnumerable<FileCabinetRecord> records);
            if (!result)
            {
                return;
            }

            if (records != null)
            {
                var recordsToDelete = records.ToArray();
                RemoveRecords(recordsToDelete);
                Print.OperationResult(recordsToDelete, DeleteCommand);
            }
        }

        private static void RemoveRecords(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                service.Remove(item.Id);
            }
        }

        private static bool IsCorrectWhereParameter(string parameters)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(parameters);
            if (isEmpty)
            {
                Print.ParametrizedCommandHint();
                return false;
            }

            bool isCorrect = !isEmpty && parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (!isCorrect)
            {
                Print.IncorrectSyntax(parameters);
            }

            return isCorrect;
        }
    }
}
