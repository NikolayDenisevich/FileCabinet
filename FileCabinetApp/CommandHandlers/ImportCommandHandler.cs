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
        private static readonly string[] PropertiesNames = new string[] { CsvPropetyName, XmlPropetyName };
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public ImportCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'import' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(ImportCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                string property = commandRequest.Parameters;
                if (Parser.TryParseImportExportParameters(PropertiesNames, ref property, out string propertyParameters))
                {
                    ImportFromFile(property, propertyParameters);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void ImportFromFile(string propertyName, string propertyParameters)
        {
            if (!IsPathValid(propertyParameters, out string filePath))
            {
                return;
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            FileCabinetServiceSnapshot snapshot = service.MakeSnapshot(records);
            using FileStream fileStream = File.OpenRead(filePath);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Unicode);
            LoadFromFile(propertyName, snapshot, streamReader);
            int restoredRecordsCount = service.Restore(snapshot);
            Console.WriteLine($"{restoredRecordsCount} records were imported from {filePath}.");
        }

        private static void LoadFromFile(string propertyName, FileCabinetServiceSnapshot snapshot, StreamReader reader)
        {
            Tuple<string, Action<StreamReader>>[] loaders = new Tuple<string, Action<StreamReader>>[]
            {
                    new Tuple<string, Action<StreamReader>>(CsvPropetyName, snapshot.LoadFromCsv),
                    new Tuple<string,  Action<StreamReader>>(XmlPropetyName, snapshot.LoadFromXml),
            };
            int index = Array.FindIndex(loaders, s => s.Item1.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            loaders[index].Item2(reader);
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
