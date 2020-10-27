using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

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
            new string[] { "import", "imports records from scpecified file: 'csv', 'xml'.", @"The 'import csv c:\folder\filename.csv' export all records from 'c:\folder\filename.csv' file." },
            new string[] { "remove", "removes record whith scpecified id", "The 'remove 1' removes an existing record #1." },
            new string[] { "purge", "purges the data file", "The 'purge' command removes all empty entries and defragment the data file (it works only for FileSystem storage.)." },
        };

        /// <summary>
        /// Handles the 'help' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
