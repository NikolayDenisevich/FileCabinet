using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides UI for file cabinet application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikolay Denisevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DefaultRootDirectory = @"C:\Cabinet"; // TODO: Fix this path.
        private const string DefaultBinaryFileName = "cabinet-records.db";
        private const string ValidationConfigFileName = "validation-rules.json";

        private static bool isRunning = true;
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService;
        private static IRecordValidator<RecordArguments> recordValidator;
        private static InputValidator inputValidator;
        private static IConfigurationRoot configuration;

        /// <summary>
        /// An application entry point.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            LoadConfig();
            CheckCommandLineArguments(args);
            ICommandHandler commandHandler = CreateCommandHandlers();
            Console.ForegroundColor = ConsoleColor.Gray;
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
            int index = 0;
            const string DefaultServerValidationType = "default";
            const string CustomServerValidationType = "custom";
            if (args is null || args.Length == 0)
            {
                fileCabinetService = CreateFileCabinetMemoryServiceInstance(DefaultServerValidationType);
            }
            else
            {
                switch (args[index].ToUpperInvariant())
                {
                    case "--VALIDATION-RULES=CUSTOM":
                        {
                            var service = CreateFileCabinetMemoryServiceInstance(CustomServerValidationType);
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    case "--VALIDATION-RULES=DEFAULT":
                        {
                            var service = CreateFileCabinetMemoryServiceInstance(DefaultServerValidationType);
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    case "-V":
                        {
                            string validationType = GetValidationValue(args, ++index);
                            var service = CreateFileCabinetMemoryServiceInstance(validationType);
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    case "--STORAGE=MEMORY":
                        {
                            var service = CreateFileCabinetMemoryServiceInstance(DefaultServerValidationType);
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    case "--STORAGE=FILE":
                        {
                            var service = CreateFileCabinetFileSystemServiceInstance();
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    case "-S":
                        {
                            var service = CheckServerStorageType(args, DefaultServerValidationType, ++index);
                            fileCabinetService = CheckNextArg(args, ++index, service);
                            break;
                        }

                    default:
                        {
                            var service = CreateFileCabinetMemoryServiceInstance(DefaultServerValidationType);
                            fileCabinetService = CheckNextArg(args, index, service);
                            break;
                        }
                }
            }
        }

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> CheckNextArg(string[] args, int nextIndex, IFileCabinetService<FileCabinetRecord, RecordArguments> service)
        {
            IFileCabinetService<FileCabinetRecord, RecordArguments> nextService;
            const string StopWatch = "use-stopwatch";
            const string Logger = "use-logger";
            if (nextIndex > args.Length - 1)
            {
                nextService = service;
            }
            else if (args[nextIndex].Equals(StopWatch, StringComparison.InvariantCultureIgnoreCase))
            {
                nextService = new ServiceMeter(service);
                Console.WriteLine("StopWatch activated.");
            }
            else if (args[nextIndex].Equals(Logger, StringComparison.InvariantCultureIgnoreCase))
            {
                nextService = new ServiceLogger(new ServiceMeter(service));
                Console.WriteLine("Logging activated.");
            }
            else
            {
                nextService = service;
            }

            return nextService;
        }

        private static string GetValidationValue(string[] args, int index)
        {
            const string DefaultServerValidationType = "DEFAULT";
            const string CustomServerValidationType = "CUSTOM";
            string result;
            if (index > args.Length - 1)
            {
                Console.WriteLine("<-v> parameter value is wrong or empty. Use <-v custom> or <--validation-rules=custom> " +
                    "if you want to use custom validation rules (custom validation rules are for memory storage only.).");
                result = DefaultServerValidationType;
            }
            else if (args[index].Equals(DefaultServerValidationType, StringComparison.InvariantCultureIgnoreCase))
            {
                result = DefaultServerValidationType;
            }
            else if (args[index].Equals(CustomServerValidationType, StringComparison.InvariantCultureIgnoreCase))
            {
                result = CustomServerValidationType;
            }
            else
            {
                Console.WriteLine("<-v> parameter value is wrong or empty. Use <-v custom> or <--validation-rules=custom> " +
                    "if you want to use custom validation rules (custom validation rules are for memory storage only.).");
                result = DefaultServerValidationType;
            }

            return result;
        }

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> CheckServerStorageType(string[] args, string serverValidationType, int index)
        {
            IFileCabinetService<FileCabinetRecord, RecordArguments> service;
            const string MemoryStorage = "MEMORY";
            const string FilesystemStorage = "FILE";
            if (index > args.Length - 1)
            {
                Console.WriteLine("<-s> parameter value is wrong or empty. Use <-s file> or <--storage=file> if you want to use filesystem storage.");
                service = CreateFileCabinetMemoryServiceInstance(serverValidationType);
            }
            else if (args[1].Equals(MemoryStorage, StringComparison.InvariantCultureIgnoreCase))
            {
                service = CreateFileCabinetMemoryServiceInstance(serverValidationType);
            }
            else if (args[1].Equals(FilesystemStorage, StringComparison.InvariantCultureIgnoreCase))
            {
                service = CreateFileCabinetFileSystemServiceInstance();
            }
            else
            {
                Console.WriteLine("<-s> parameter value is wrong or empty. Use <-s file> or <--storage=file> if you want to use filesystem storage.");
                service = CreateFileCabinetMemoryServiceInstance(serverValidationType);
            }

            return service;
        }

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> CreateFileCabinetFileSystemServiceInstance()
        {
            const string serverValidationType = "default";
            Console.WriteLine($"Using {serverValidationType} validation rules.");
            Console.WriteLine("Using filesystem storage.");
            InitializeValidators(serverValidationType);
            string fullPath = Path.Combine(DefaultRootDirectory, DefaultBinaryFileName);
            FileStream fileStream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return new FileCabinetFilesystemService(recordValidator, fileStream);
        }

        private static void LoadConfig()
        {
            string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), ValidationConfigFileName);
            if (File.Exists(configFilePath))
            {
                 configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ValidationConfigFileName)
                .Build();
            }
            else
            {
                Console.WriteLine("Configuration file not found. Application will be closed.");
                Environment.Exit(-1);
            }
        }

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> CreateFileCabinetMemoryServiceInstance(string serverValidationType)
        {
            Console.WriteLine($"Using {serverValidationType} validation rules.");
            Console.WriteLine("Using memory storage.");
            InitializeValidators(serverValidationType);
            return new FileCabinetMemoryService(recordValidator);
        }

        private static void InitializeValidators(string serverValidationType)
        {
            var defaultSection = configuration.GetSection(serverValidationType);
            var validationRules = new ValidationRulesContainer(defaultSection);
            recordValidator = new ValidatorBuilder().CreateValidator(validationRules);
            inputValidator = new InputValidator(validationRules);
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