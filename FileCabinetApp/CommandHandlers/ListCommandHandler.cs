using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'list' command handler for FileCabinetApp.
    /// </summary>
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string ListCommand = "list";
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;
        private static Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="recordPrinter">Record printer instance.</param>
        public ListCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
        {
            service = fileCabinetService;
            ListCommandHandler.recordPrinter = recordPrinter;
        }

        /// <summary>
        /// Handles the 'list' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(ListCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                List();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void List()
        {
            IReadOnlyCollection<FileCabinetRecord> readOnlyCollection = service.GetRecords();
            if (readOnlyCollection.Count != 0)
            {
                recordPrinter(readOnlyCollection);
            }
            else
            {
                Console.WriteLine("There is no records in list");
            }
        }
    }
}
