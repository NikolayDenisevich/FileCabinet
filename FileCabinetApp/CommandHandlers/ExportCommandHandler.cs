using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'export' command handler for FileCabinetApp.
    /// </summary>
    internal class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string ExportCommand = "export";
        private const string CsvPropetyName = "csv";
        private const string XmlPropetyName = "xml";
        private static readonly string[] PropertiesNew = new string[] { CsvPropetyName, XmlPropetyName };
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public ExportCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'export' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(ExportCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                string property = commandRequest.Parameters;
                if (Parser.TryParseImportExportParameters(PropertiesNew, ref property, out string propertyParameters))
                {
                    ExportInFile(property, propertyParameters);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void ExportInFile(string propertyName, string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine($"'{propertyName}' value is empty.");
                return;
            }

            if (service.GetStat(out int _) is 0)
            {
                Console.WriteLine("There is nothing to export. Records count is 0.");
                return;
            }

            string filePath = parameters.Trim();
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                CheckFileAndSave(filePath, propertyName);
            }
            else if (Directory.Exists(directoryPath))
            {
                CheckFileAndSave(filePath, propertyName);
            }
            else
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
        }

        private static void CheckFileAndSave(string filePath, string propertyName)
        {
            if (File.Exists(filePath))
            {
                bool isRewriteTrue = ReadInputForRewrite(filePath);
                if (!isRewriteTrue)
                {
                    return;
                }
            }

            SaveToFile(filePath, propertyName);
        }

        private static bool ReadInputForRewrite(string filePath)
        {
            Console.Write($"File is exist - rewrite {filePath}? [Y/n] : ");
            bool isCorrect = false;
            bool isRewriteTrue = true;
            while (!isCorrect)
            {
                string input = Console.ReadLine().ToUpperInvariant();
                switch (input)
                {
                    case "Y":
                        {
                            isCorrect = true;
                            isRewriteTrue = true;
                            break;
                        }

                    case "N":
                        {
                            isCorrect = true;
                            isRewriteTrue = false;
                            break;
                        }

                    default:
                        Console.Write("Enter [y/n]");
                        break;
                }
            }

            return isRewriteTrue;
        }

        private static void SaveToFile(string filePath, string propertyName)
        {
            var records = new ReadOnlyCollection<FileCabinetRecord>(service.GetRecords(null, null).ToList());
            FileCabinetServiceSnapshot snapshot = service.MakeSnapshot(records);
            using var streamWriter = new StreamWriter(filePath, false, Encoding.Unicode);
            SelectDestination(propertyName, snapshot, streamWriter);
            Console.WriteLine($"All records are exported to file {filePath}.");
        }

        private static void SelectDestination(string propertyName, FileCabinetServiceSnapshot snapshot, StreamWriter writer)
        {
            Tuple<string, Action<StreamWriter>>[] savers = new Tuple<string, Action<StreamWriter>>[]
            {
                    new Tuple<string, Action<StreamWriter>>(CsvPropetyName, snapshot.SaveToCsv),
                    new Tuple<string,  Action<StreamWriter>>(XmlPropetyName, snapshot.SaveToXml),
            };
            int index = Array.FindIndex(savers, s => s.Item1.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            savers[index].Item2(writer);
        }
    }
}
