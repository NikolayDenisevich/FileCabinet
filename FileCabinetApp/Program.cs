using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides UI for file cabinet application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikolay Denisevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string DefaultRootDirectory = @"C:\Cabinet";

        private static bool isRunning = true;
        private static IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService;
        private static IRecordValidator<RecordArguments> recordValidator;
        private static InputValidator inputValidator;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static Tuple<string, string, Action<string, string>>[] properties = new Tuple<string, string, Action<string, string>>[]
        {
            new Tuple<string, string, Action<string, string>>("firstname", "First Name", ShowByFirstName),
            new Tuple<string, string, Action<string, string>>("lastname", "Last Name", ShowByLastName),
            new Tuple<string, string, Action<string, string>>("dateofbirth", "Date of birth", ShowByDate),
            new Tuple<string, string, Action<string, string>>("csv", "csv export", ExportToCsv),
            new Tuple<string, string, Action<string, string>>("xml", "xml export", ExportToXml),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' creates a new record." },
            new string[] { "list", "displays a list of records added to the service.", "The 'list' displays a list of records added to the service." },
            new string[] { "edit", "edits an existing record.", "The 'edit 1' edits an existing record #1." },
            new string[] { "find", "finds records with a scpecified properties: 'firstname', 'lastname' or 'dateofbirth'.", "The 'find firstname Petr' serches all records with firstname Petr." },
            new string[] { "export", "exports records to scpecified file: 'csv', 'xml'.", "The 'export csv filename.csv' export all records to 'filename.csv' file." },
        };

        /// <summary>
        /// An application entry point.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            CheckCommandLineArguments(args);
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            RecordArguments arguments = ReadArguments();
            int id = fileCabinetService.CreateRecord(arguments);
            Console.WriteLine($"Record #{id} is created");
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> readOnlyCollection = fileCabinetService.GetRecords();
            if (readOnlyCollection.Count != 0)
            {
                ShowRecords(readOnlyCollection);
            }
            else
            {
                Console.WriteLine("There is no records in list");
            }
        }

        private static void Edit(string parameters)
        {
            int id;
            bool isParsed = int.TryParse(parameters, out id);
            if (!isParsed || id == 0)
            {
                Console.WriteLine($"Record is not found.");
                return;
            }

            ReadOnlyCollection<FileCabinetRecord> readonlyCollection = fileCabinetService.GetRecords();
            bool isExists = IsExistsRecordIdInList(id, readonlyCollection);
            if (!isExists)
            {
                Console.WriteLine($"Record #{id} is not found.");
                return;
            }

            RecordArguments arguments = ReadArguments();
            arguments.Id = id;

            fileCabinetService.EditRecord(arguments);
            Console.WriteLine($"Record #{id} is updated");
        }

        private static RecordArguments ReadArguments()
        {
            Console.Write("First Name: ");
            string firstName = ReadInput(StringsConverter, inputValidator.ValidateStrings);
            Console.Write("Last Name: ");
            string lastName = ReadInput(StringsConverter, inputValidator.ValidateStrings);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(DatesConverter, inputValidator.ValidateDate);
            Console.Write("Zip code: ");
            short zipCode = ReadInput(ShortConverter, inputValidator.ValidateShort);
            Console.Write("City: ");
            string city = ReadInput(StringsConverter, inputValidator.ValidateStrings);
            Console.Write("Street: ");
            string street = ReadInput(StringsConverter, inputValidator.ValidateStrings);
            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, inputValidator.ValidateDecimal);
            Console.Write("Gender: ");
            char gender = ReadInput(CharConverter, inputValidator.ValidateChar);

            var arguments = new RecordArguments
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ZipCode = zipCode,
                City = city,
                Street = street,
                Salary = salary,
                Gender = gender,
            };
            return arguments;
        }

        private static void Find(string parameters)
        {
            ParametersParser(parameters);
        }

        private static void ParametersParser(string parameters)
        {
            var inputs = parameters.Split(' ', 2);
            const int propertyIndex = 0;
            string property = inputs[propertyIndex];

            if (string.IsNullOrEmpty(property))
            {
                Console.WriteLine(Program.HintMessage);
                return;
            }

            int index = Array.FindIndex(properties, 0, properties.Length, i => i.Item1.Equals(property, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int valueIndex = 1;
                string propertyValue = inputs.Length > 1 ? inputs[valueIndex] : string.Empty;
                properties[index].Item3(properties[index].Item2, propertyValue);
            }
            else
            {
                PrintMissedPropertyInfo(property);
            }
        }

        private static void Export(string parameters)
        {
            ParametersParser(parameters);
        }

        private static void ExportToXml(string propertyFullName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"'{propertyFullName}' value is empty.");
                return;
            }

            string filePath = value.Trim();
            string directoryPath = Path.GetDirectoryName(filePath);
            bool isRewriteTrue;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(DefaultRootDirectory, filePath);
                if (File.Exists(filePath))
                {
                    isRewriteTrue = ReadInputForRewrite(filePath);
                    if (!isRewriteTrue)
                    {
                        return;
                    }
                }

                SaveToXml(filePath);
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

                SaveToXml(filePath);
            }
            else
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
        }

        private static void SaveToXml(string filePath)
        {
            FileCabinetService servise = fileCabinetService as FileCabinetService;
            var records = servise.GetRecords();
            FileCabinetServiceSnapshot snapshot = FileCabinetService.MakeSnapshot(records);
            using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                snapshot.SaveToXml(streamWriter);
                Console.WriteLine($"All records are exported to file {filePath}.");
            }
        }

        private static void ExportToCsv(string propertyFullName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"'{propertyFullName}' value is empty.");
                return;
            }

            string filePath = value.Trim();
            string directoryPath = Path.GetDirectoryName(filePath);
            bool isRewriteTrue;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(DefaultRootDirectory, filePath);
                if (File.Exists(filePath))
                {
                    isRewriteTrue = ReadInputForRewrite(filePath);
                    if (!isRewriteTrue)
                    {
                        return;
                    }
                }

                SaveToCsv(filePath);
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

                SaveToCsv(filePath);
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

        private static void SaveToCsv(string filePath)
        {
            FileCabinetService servise = fileCabinetService as FileCabinetService;
            var records = servise.GetRecords();
            FileCabinetServiceSnapshot snapshot = FileCabinetService.MakeSnapshot(records);
            using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                snapshot.SaveToCsv(streamWriter);
                Console.WriteLine($"All records are exported to file {filePath}.");
            }
        }

        private static void PrintMissedPropertyInfo(string propertyName)
        {
            Console.WriteLine($"There is no '{propertyName}' property.");
            Console.WriteLine();
        }

        private static void ShowByFirstName(string propertyFullName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string trimmedValue = value.Trim();
                FileCabinetService service = fileCabinetService as FileCabinetService;
                ReadOnlyCollection<FileCabinetRecord> readonlyCollection = service.FindByFirstName(trimmedValue);
                CheckRecordsForZeroOrShow(readonlyCollection, propertyFullName, trimmedValue);
            }
            else
            {
                Console.WriteLine($"'{propertyFullName}' value is empty.");
            }
        }

        private static void ShowByLastName(string propertyFullName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string trimmedValue = value.Trim();
                FileCabinetService service = fileCabinetService as FileCabinetService;
                ReadOnlyCollection<FileCabinetRecord> readonlyCollection = service.FindByLastName(trimmedValue);
                CheckRecordsForZeroOrShow(readonlyCollection, propertyFullName, trimmedValue);
            }
            else
            {
                Console.WriteLine($"'{propertyFullName}' value is empty.");
            }
        }

        private static void ShowByDate(string propertyFullName, string value)
        {
            DateTime dateOfBirth;
            if (!string.IsNullOrEmpty(value))
            {
                string trimmedValue = value.Trim();
                bool isParsed = DateTime.TryParse(trimmedValue, out dateOfBirth);
                if (isParsed)
                {
                    FileCabinetService service = fileCabinetService as FileCabinetService;
                    ReadOnlyCollection<FileCabinetRecord> readonlyCollection = service.FindByDateOfBirth(dateOfBirth);
                    CheckRecordsForZeroOrShow(readonlyCollection, propertyFullName, value);
                }
                else
                {
                    Console.WriteLine($"There is no records with '{trimmedValue}' {propertyFullName}");
                }
            }
            else
            {
                Console.WriteLine($"'{propertyFullName}' value is empty.");
            }
        }

        private static void ShowRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            foreach (var item in records)
            {
                Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, {item.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, " +
                    $"{item.ZipCode}, {item.City}, {item.Street}, {item.Salary}, {item.Gender}");
            }
        }

        private static void CheckRecordsForZeroOrShow(ReadOnlyCollection<FileCabinetRecord> records, string propertyName, string input)
        {
            if (records.Count == 0)
            {
                Console.WriteLine($"There is no records with {propertyName} '{input}'");
            }
            else
            {
                ShowRecords(records);
            }
        }

        private static bool IsExistsRecordIdInList(int id, ReadOnlyCollection<FileCabinetRecord> collection)
        {
            bool isExists = false;

            foreach (var item in collection)
            {
                if (item.Id == id)
                {
                    isExists = true;
                    break;
                }
            }

            return isExists;
        }

        private static void CheckCommandLineArguments(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                CreateFileCabinerDefaultServiceInstance();
            }
            else
            {
                switch (args[0].ToUpperInvariant())
                {
                    case "--VALIDATION-RULES=CUSTOM":
                        {
                            CreateFileCabinerCustomServiceInstance();
                            break;
                        }

                    case "--VALIDATION-RULES=DEFAULT":
                        {
                            CreateFileCabinerDefaultServiceInstance();
                            break;
                        }

                    case "-V":
                        {
                            if (args.Length < 2 || args[1].Equals("default", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinerDefaultServiceInstance();
                                break;
                            }

                            if (args[1].Equals("custom", StringComparison.InvariantCultureIgnoreCase))
                            {
                                CreateFileCabinerCustomServiceInstance();
                                break;
                            }
                            else
                            {
                                CreateFileCabinerDefaultServiceInstance();
                                break;
                            }
                        }

                    default:
                        {
                            CreateFileCabinerDefaultServiceInstance();
                            break;
                        }
                }
            }
        }

        private static void CreateFileCabinerDefaultServiceInstance()
        {
            Console.WriteLine("Using default validation rules.");
            recordValidator = new DefaultValidator();
            inputValidator = recordValidator.GetInputValidator();
            fileCabinetService = new FileCabinetService(recordValidator);
        }

        private static void CreateFileCabinerCustomServiceInstance()
        {
            Console.WriteLine("Using custom validation rules.");
            recordValidator = new CustomValidator();
            inputValidator = recordValidator.GetInputValidator();
            fileCabinetService = new FileCabinetService(recordValidator);
        }

        private static Tuple<bool, string, string> StringsConverter(string input)
        {
            return new Tuple<bool, string, string>(true, string.Empty, input.Trim());
        }

        private static Tuple<bool, string, DateTime> DatesConverter(string input)
        {
            DateTime dateOfBirth;
            bool isParsed = DateTime.TryParse(input.Trim(), out dateOfBirth);
            string message = "Correct format is: dd/mm/yyyy";
            return new Tuple<bool, string, DateTime>(isParsed, message, dateOfBirth);
        }

        private static Tuple<bool, string, short> ShortConverter(string input)
        {
            short zipCode;
            bool isParsed = short.TryParse(input.Trim(), out zipCode);
            string message = "Correct format is: 1-4-digit numbers only";
            return new Tuple<bool, string, short>(isParsed, message, zipCode);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            decimal salary;
            bool isParsed = decimal.TryParse(input.Trim(), out salary);
            string message = "Correct format is: decimal numbers.";
            return new Tuple<bool, string, decimal>(isParsed, message, salary);
        }

        private static Tuple<bool, string, char> CharConverter(string input)
        {
            char gender;
            bool isParsed = char.TryParse(input.Trim(), out gender);
            string message = "Correct format is: one character only";
            return new Tuple<bool, string, char>(isParsed, message, gender);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}