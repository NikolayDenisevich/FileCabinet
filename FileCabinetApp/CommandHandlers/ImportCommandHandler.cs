using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'import' command handler for FileCabinetApp.
    /// </summary>
    internal class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string ImportCommand = "import";
        private const string CsvPropetyName = "csv";
        private const string XmlPropetyName = "xml";
        private static Tuple<string, Action<string, string>>[] properties = new Tuple<string, Action<string, string>>[]
        {
            new Tuple<string, Action<string, string>>(CsvPropetyName, ImportFromFile),
            new Tuple<string, Action<string, string>>(XmlPropetyName, ImportFromFile),
        };

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        public ImportCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'import' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(ImportCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Parser.ParceParameters(properties, commandRequest.Parameters, ' ');
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void ImportFromFile(string propertyName, string value)
        {
            if (!IsPathValid(value, out string filePath))
            {
                return;
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            FileCabinetServiceSnapshot snapshot = service.MakeSnapshot(records);
            using FileStream fileStream = File.OpenRead(filePath);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Unicode);
            SelectSoruce(propertyName, snapshot, streamReader);
            int restoredRecordsCount = service.Restore(snapshot);
            streamReader.Close();
            fileStream.Close();
            Console.WriteLine($"{restoredRecordsCount} records were imported from {filePath}.");
        }

        private static void SelectSoruce(string propertyName, FileCabinetServiceSnapshot snapshot, StreamReader reader)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            string propertyInLower = propertyName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            switch (propertyInLower)
            {
                case CsvPropetyName:
                    {
                        snapshot.LoadFromCsv(reader);
                        break;
                    }

                case XmlPropetyName:
                    {
                        snapshot.LoadFromXml(reader);
                        break;
                    }
            }
        }

        private static bool IsPathValid(string value, out string filePath)
        {
            filePath = string.Empty;
            bool isValid = true;
            if (string.IsNullOrEmpty(value))
            {
                Print.ParametrizedCommandHint();
                isValid = false;
            }
            else
            {
                filePath = value.Trim();
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File '{filePath}' does not exist.");
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
