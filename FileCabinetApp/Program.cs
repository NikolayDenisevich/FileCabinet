using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides UI for file cabinet application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikolay Denisevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DefaultRootDirectory = @"C:\Cabinet";
        private const string DefaultBinaryFileName = "cabinet-records.db";

        private static bool isRunning = true;
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService;
        private static IRecordValidator<RecordArguments> recordValidator;
        private static InputValidator inputValidator;

        /// <summary>
        /// An application entry point.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            CheckCommandLineArguments(args);
            ICommandHandler commandHandler = CreateCommandHandlers();
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                string[] inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                const int parametersIndex = 1;
                string command = inputs[commandIndex];
                string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static void CheckCommandLineArguments(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                CreateFileCabinetDefaultServiceInstance(); // TODO:

                // CreateFileCabinetFileSystemServiceInstance();
            }
            else
            {
                switch (args[0].ToUpperInvariant())
                {
                    case "--VALIDATION-RULES=CUSTOM":
                        {
                            CreateFileCabinetCustomValidationServiceInstance();
                            break;
                        }

                    case "--VALIDATION-RULES=DEFAULT":
                        {
                            CreateFileCabinetDefaultServiceInstance();
                            break;
                        }

                    case "-V":
                        {
                            if (args.Length < 2 || args[1].Equals("default", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinetDefaultServiceInstance();
                                break;
                            }

                            if (args[1].Equals("custom", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinetCustomValidationServiceInstance();
                                break;
                            }
                            else
                            {
                                CreateFileCabinetDefaultServiceInstance();
                                break;
                            }
                        }

                    case "--STORAGE=MEMORY":
                        {
                            CreateFileCabinetDefaultServiceInstance();
                            break;
                        }

                    case "--STORAGE=FILE":
                        {
                            CreateFileCabinetFileSystemServiceInstance();
                            break;
                        }

                    case "-S":
                        {
                            if (args.Length < 2 || args[1].Equals("memory", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinetDefaultServiceInstance();
                                break;
                            }

                            if (args[1].Equals("file", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinetFileSystemServiceInstance();
                                break;
                            }
                            else
                            {
                                CreateFileCabinetDefaultServiceInstance();
                                break;
                            }
                        }

                    default:
                        {
                            CreateFileCabinetDefaultServiceInstance();
                            break;
                        }
                }
            }
        }

        private static void CreateFileCabinetFileSystemServiceInstance()
        {
            Console.WriteLine("Using default validation rules.");
            Console.WriteLine("Using filesystem storage.");
            recordValidator = new ValidatorBuilder().CreateDefaultValidator();
            inputValidator = new DefaultInputValidator();
            string fullPath = Path.Combine(DefaultRootDirectory, DefaultBinaryFileName);
            FileStream fileStream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileCabinetService = new FileCabinetFilesystemService(recordValidator, fileStream);
        }

        private static void CreateFileCabinetDefaultServiceInstance()
        {
            Console.WriteLine("Using default validation rules.");
            Console.WriteLine("Using memory storage.");
            recordValidator = new ValidatorBuilder().CreateDefaultValidator();
            inputValidator = new DefaultInputValidator();
            fileCabinetService = new FileCabinetMemoryService(recordValidator);
        }

        private static void CreateFileCabinetCustomValidationServiceInstance()
        {
            Console.WriteLine("Using custom validation rules.");
            recordValidator = new ValidatorBuilder().CreateCustomValidator();
            inputValidator = new CustomInputValidator();
            fileCabinetService = new FileCabinetMemoryService(recordValidator);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            IRecordPrinter<FileCabinetRecord> recordPrinter = new DefaultRecordPrinter();
            var createHandler = new CreateCommandHandler(fileCabinetService, inputValidator);
            var editHandler = new EditCommandHandler(fileCabinetService, inputValidator);
            var exitHandler = new ExitCommandHandler(r => isRunning = r);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint, inputValidator);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);

            createHandler
                .SetNext(editHandler)
                .SetNext(exitHandler)
                .SetNext(exportHandler)
                .SetNext(findHandler)
                .SetNext(helpHandler)
                .SetNext(importHandler)
                .SetNext(listHandler)
                .SetNext(purgeHandler)
                .SetNext(removeHandler)
                .SetNext(statHandler);

            return createHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null.");
            }

            foreach (var item in records)
            {
                Console.WriteLine(item);
            }
        }
    }
}