using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
        public DeleteCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'delete' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(DeleteCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                DeleteNew(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void DeleteNew(string parameters)
        {
            bool result = IsCorrectWhereParameter(parameters);
            if (!result)
            {
                return;
            }

            parameters = parameters.Replace(WhereLiteral, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
            var filtersDictionary = new Dictionary<string, List<object>>(StringComparer.InvariantCultureIgnoreCase);
            result = Parser.TryParceFilters(parameters, filtersDictionary, out bool isAndAlsoCombineMethod);
            if (!result)
            {
                return;
            }

            var filtersPredicate = PredicatesFactory.GetPredicate(filtersDictionary, isAndAlsoCombineMethod);
            IEnumerable<FileCabinetRecord> records;
            if (filtersPredicate is null)
            {
                return;
            }

            records = service.GetRecords(parameters, filtersPredicate);
            if (!records.Any())
            {
                Print.NoRecordsWithFilters(parameters);
                return;
            }

            var recordsToDelete = records.ToArray();
            RemoveRecords(recordsToDelete);
            Print.OperationResult(recordsToDelete, DeleteCommand);
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
