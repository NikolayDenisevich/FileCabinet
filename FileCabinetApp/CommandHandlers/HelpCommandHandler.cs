using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides 'help' command handler for FileCabinetApp.
    /// </summary>
    internal class HelpCommandHandler : CommandHandlerBase
    {
        private const string HelpCommand = "help";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string InsertCommandUsage = "insert (id, firstname, lastname, dateofbirth, zipcode, city, street, salary, gender) " +
            "values ('1', 'John', 'Doe', '18/5/1986', '6548', 'min', 'baki', '6548.5', 'f')";

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' creates a new record." },
            new string[] { "export", "exports records to scpecified file: 'csv', 'xml'.", "The 'export csv filename.csv' export all records to 'filename.csv' file." },
            new string[] { "import", "imports records from scpecified file: 'csv', 'xml'.", @"The 'import csv c:\folder\filename.csv' export all records from 'c:\folder\filename.csv' file." },
            new string[] { "purge", "purges the data file", "The 'purge' command removes all empty entries and defragment the data file (it works only for FileSystem storage.)." },
            new string[]
                         {
                             "insert", "adds new record using specified data", $"The '{InsertCommandUsage}' command " +
                             "adds new record with Id=1, firstname 'John', lastname 'Doe', date of birth 18/05/1986 etc...",
                         },
            new string[] { "delete", "deletes records using specified criteria", "The 'delete where id = '1' deletes record #1." },
            new string[] { "update", "updates records using specified criteria", "Using exapmle: 'update set firstname = 'John', lastname = 'Doe' , dateofbirth = '18/05/1986' where id = '1'" },
            new string[] { "select", "selects records using specified criteria", "Using exapmle: 'select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'" },
        };

        /// <summary>
        /// Handles the 'help' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (commandRequest.Command.Equals(HelpCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintHelp(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
