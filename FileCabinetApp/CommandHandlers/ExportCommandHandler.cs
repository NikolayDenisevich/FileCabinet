using System;
using System.IO;
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

        private static Tuple<string, Action<string, string>>[] properties = new Tuple<string, Action<string, string>>[]
        {
            new Tuple<string, Action<string, string>>(CsvPropetyName, ExportInFile),
            new Tuple<string,  Action<string, string>>(XmlPropetyName, ExportInFile),
        };

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        public ExportCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            service = fileCabinetService;
        }

        /// <summary>
        /// Handles the 'export' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(ExportCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                ParceParameters(properties, commandRequest.Parameters, ' ');
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void ExportInFile(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"'{propertyName}' value is empty.");
                return;
            }

            if (service.GetStat().Item1 is 0)
            {
                Console.WriteLine("There is nothing to export. Records count is 0.");
                return;
            }

            string filePath = value.Trim();
            string directoryPath = Path.GetDirectoryName(filePath);
            bool isRewriteTrue;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (File.Exists(filePath))
                {
                    isRewriteTrue = ReadInputForRewrite(filePath);
                    if (!isRewriteTrue)
                    {
                        return;
                    }
                }

                SaveToFile(filePath, propertyName);
            }
            else if (Directory.Exists(directoryPath))
            {
                if (File.Exists(filePath))
                {
                    isRewriteTrue = ReadInputForRewrite(filePath);
                    if (!isRewriteTrue)
                    {
                        return;
                    }
                }

                SaveToFile(filePath, propertyName);
            }
            else
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
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
            var records = service.GetRecords();
            FileCabinetServiceSnapshot snapshot = service.MakeSnapshot(records);
            using (var streamWriter = new StreamWriter(filePath, false, Encoding.Unicode))
            {
                SelectDestination(propertyName, snapshot, streamWriter);
                Console.WriteLine($"All records are exported to file {filePath}.");
            }
        }

        private static void SelectDestination(string propertyName, FileCabinetServiceSnapshot snapshot, StreamWriter writer)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            string propertyInLower = propertyName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            switch (propertyInLower)
            {
                case CsvPropetyName:
                    {
                        snapshot.SaveToCsv(writer);
                        break;
                    }

                case XmlPropetyName:
                    {
                        snapshot.SaveToXml(writer);
                        break;
                    }
            }
        }
    }
}
