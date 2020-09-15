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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'stat' creates a new record." },
            new string[] { "list", "displays a list of records added to the service.", "The 'list' displays a list of records added to the service." },
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
            Console.Write("First Name: ");
            string firsName = Console.ReadLine();
            NamesInputCheck(firsName, "First Name");
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();
            NamesInputCheck(lastName, "Last Name");
            Console.Write("Date of birth: ");
            string dateOfBirth = Console.ReadLine();
            var dateTimeBirth = DateOfBirthInputCheck(dateOfBirth);
            Console.Write("ZIP Code: ");
            short zipCode;
            _ = short.TryParse(Console.ReadLine(), out zipCode);
            Console.Write("City: ");
            string city = Console.ReadLine();
            Console.Write("Street: ");
            string street = Console.ReadLine();
            Console.Write("Salary: ");
            decimal salary;
            _ = decimal.TryParse(Console.ReadLine(), out salary);
            Console.Write("Gender: ");
            char gender;
            _ = char.TryParse(Console.ReadLine(), out gender);
            Console.WriteLine($"Record #{fileCabinetService.CreateRecord(firsName, lastName, dateTimeBirth, zipCode, city, street, salary, gender)} is created");
        }

        private static string NamesInputCheck(string input, string parameterName)
        {
            while (input.Length == 0)
            {
                Console.WriteLine($"{parameterName} cannot be empty, please try again:");
                input = Console.ReadLine();
            }

            return input;
        }

        private static DateTime DateOfBirthInputCheck(string input)
        {
            bool isParced;
            DateTime dateTimeBirth;
            do
            {
                isParced = DateTime.TryParse(input, out dateTimeBirth);

                if (!isParced)
                {
                    Console.WriteLine("Invalid format: Date of birth");
                    Console.WriteLine("Correct format is: dd/mm/yyyy");
                    Console.Write("Please, repeat:");
                    input = Console.ReadLine();
                }
            }
            while (!isParced);
            return dateTimeBirth;
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] list = fileCabinetService.GetRecords();
            if (list.Length != 0)
            {
                foreach (var item in list)
                {
                    Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, {item.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, " +
                        $"{item.ZipCode}, {item.City}, {item.Street}, {item.Salary}? {item.Gender}");
                }
            }
            else
            {
                Console.WriteLine("There is no records in list");
            }
        }
    }
}