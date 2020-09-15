using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Nikolay Denisevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static Tuple<string, string, Action<string, string>>[] properties = new Tuple<string, string, Action<string, string>>[]
        {
            new Tuple<string, string, Action<string, string>>("firstname", "First Name", ShowByFirstName),
            new Tuple<string, string, Action<string, string>>("lastname", "Last Name", ShowByLastName),
            new Tuple<string, string, Action<string, string>>("dateofbirth", "Date of birth", ShowByDate),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' creates a new record." },
            new string[] { "list", "displays a list of records added to the service.", "The 'list' displays a list of records added to the service." },
            new string[] { "edit", "edits an existing record.", "The 'edit 1' edits an existing record #1." },
            new string[] { "find", "finds records with a scpecified firstname or lastname.", "The 'find firstname Petr' serches all records with firstname Petr." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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
            string firstName = ReadNames("First Name: ");
            string lastName = ReadNames("Last Name: ");
            DateTime dateTimeBirth = ReadDate("Date of birth: ");
            short zipCode = ReadZipCode("ZIP Code: ");
            string city = ReadNames("City: ");
            string street = ReadNames("Street: ");
            decimal salary = ReadSalary("Salary: ");
            char gender = ReadGender("Gender: ");
            Console.WriteLine($"Record #{fileCabinetService.CreateRecord(firstName, lastName, dateTimeBirth, zipCode, city, street, salary, gender)} is created");
        }

        private static void Edit(string parameters)
        {
            int id;
            if (!int.TryParse(parameters, out id) || id == 0)
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            FileCabinetRecord[] list = fileCabinetService.GetRecords();
            bool isFound = false;
            foreach (var item in list)
            {
                if (item.Id == id)
                {
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            string firstName = ReadNames("First Name: ");
            string lastName = ReadNames("Last Name: ");
            DateTime dateTimeBirth = ReadDate("Date of birth: ");
            short zipCode = ReadZipCode("ZIP Code: ");
            string city = ReadNames("City: ");
            string street = ReadNames("Street: ");
            decimal salary = ReadSalary("Salary: ");
            char gender = ReadGender("Gender: ");
            fileCabinetService.EditRecord(id, firstName, lastName, dateTimeBirth, zipCode, city, street, salary, gender);
            Console.WriteLine($"Record #{id} is updated");
        }

        private static void Find(string parameters)
        {
            var inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
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

        private static void PrintMissedPropertyInfo(string property)
        {
            Console.WriteLine($"There is no '{property}' property.");
            Console.WriteLine();
        }

        private static void ShowByFirstName(string propertyFullName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string trimmedValue = value.Trim();
                FileCabinetRecord[] records = fileCabinetService.FindByFirstName(trimmedValue);
                CheckRecordsForZeroOrShow(records, propertyFullName, trimmedValue);
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
                FileCabinetRecord[] records = fileCabinetService.FindByLastName(trimmedValue);
                CheckRecordsForZeroOrShow(records, propertyFullName, trimmedValue);
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
                if (DateTime.TryParse(trimmedValue, out dateOfBirth))
                {
                    FileCabinetRecord[] records = fileCabinetService.FindByDateOfBirth(dateOfBirth);
                    CheckRecordsForZeroOrShow(records, propertyFullName, value);
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

        private static string ReadNames(string parameterName)
        {
            Console.Write($"{parameterName}");
            string input = Console.ReadLine().Trim();
            while (string.IsNullOrWhiteSpace(input) || input.Length < 2 || input.Length > 60)
            {
                Console.WriteLine($"{parameterName}minimum number of characters is 2, maximum is 60 and cannot be empty or or contain only space characters please try again:");
                Console.Write($"{parameterName}");
                input = Console.ReadLine().Trim();
            }

            return input;
        }

        private static DateTime ReadDate(string parameterName)
        {
            Console.Write($"{parameterName}");
            string input = Console.ReadLine();
            bool isCorrectDate = false;
            DateTime dateTimeBirth;
            do
            {
                if (!DateTime.TryParse(input, out dateTimeBirth))
                {
                    Console.WriteLine("Invalid format: Date of birth");
                    Console.WriteLine("Correct format is: dd/mm/yyyy");
                    Console.WriteLine("Please, repeat:");
                    Console.Write($"{parameterName}");
                    input = Console.ReadLine();
                }
                else
                {
                    if (dateTimeBirth < new DateTime(1950, 1, 1) || dateTimeBirth > DateTime.Now)
                    {
                        Console.WriteLine($"Date of birth should be more than 01-Jan-1950 and not more than {DateTime.Now.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)}");
                        Console.WriteLine("Please, repeat input.");
                        Console.Write($"{parameterName}");
                        input = Console.ReadLine();
                    }
                    else
                    {
                        isCorrectDate = true;
                    }
                }
            }
            while (!isCorrectDate);
            return dateTimeBirth;
        }

        private static short ReadZipCode(string parameterName)
        {
            Console.Write($"{parameterName}");
            short zipCode = 0;
            bool isParsed = false;
            while (!isParsed || zipCode < 0 || zipCode > 9999)
            {
                isParsed = short.TryParse(Console.ReadLine().Trim(), out zipCode);
                if (!isParsed || zipCode < 0 || zipCode > 9999)
                {
                    Console.WriteLine($"{parameterName} range is 1..9999 or invalid input. Please repeat input.");
                    Console.Write($"{parameterName}: ");
                }
            }

            return zipCode;
        }

        private static decimal ReadSalary(string parameterName)
        {
            Console.Write($"{parameterName}");
            decimal salary = 0;
            bool isParsed = false;
            while (!isParsed || salary < 0 || salary > 100000)
            {
                isParsed = decimal.TryParse(Console.ReadLine().Trim(), out salary);
                if (!isParsed || salary < 0 || salary > 100000)
                {
                    Console.WriteLine($"{parameterName}range is 1..100 000, or invalid input. Please repeat input.");
                    Console.Write($"{parameterName}");
                }
            }

            return salary;
        }

        private static char ReadGender(string parameterName)
        {
            Console.Write($"{parameterName}");
            char gender = default;
            bool isParsed = false;
            while (!isParsed || (gender != 'm' && gender != 'M' && gender != 'f' && gender != 'F'))
            {
                isParsed = char.TryParse(Console.ReadLine().Trim(), out gender);
                if (!isParsed || (gender != 'm' && gender != 'M' && gender != 'f' && gender != 'F'))
                {
                    Console.WriteLine($"{parameterName}permissible values are :'m', 'M', 'f', 'F'. Please repeat input.");
                    Console.Write($"{parameterName}");
                }
            }

            return gender;
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] list = fileCabinetService.GetRecords();
            if (list.Length != 0)
            {
                ShowRecords(list);
            }
            else
            {
                Console.WriteLine("There is no records in list");
            }
        }

        private static void ShowRecords(FileCabinetRecord[] records)
        {
            foreach (var item in records)
            {
                Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, {item.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, " +
                    $"{item.ZipCode}, {item.City}, {item.Street}, {item.Salary}, {item.Gender}");
            }
        }

        private static void CheckRecordsForZeroOrShow(FileCabinetRecord[] records, string propertyName, string input)
        {
            if (records.Length == 0)
            {
                Console.WriteLine($"There is no records with {propertyName} '{input}'");
            }
            else
            {
                ShowRecords(records);
            }
        }
    }
}