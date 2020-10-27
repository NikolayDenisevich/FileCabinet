using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'find' command handler for FileCabinetApp.
    /// </summary>
    internal class FindCommandHandler : ServiceCommandHandlerBase
    {
        private const string FindCommand = "find";
        private const string FirstNameProperty = "firstname";
        private const string LastNameProperty = "lastname";
        private const string DateOfBirthProperty = "dateofbirth";

        private static Tuple<string, Action<string, string>>[] properties = new Tuple<string, Action<string, string>>[]
        {
            new Tuple<string, Action<string, string>>(FirstNameProperty, ShowByName),
            new Tuple<string, Action<string, string>>(LastNameProperty, ShowByName),
            new Tuple<string, Action<string, string>>(DateOfBirthProperty, ShowByDate),
        };

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;
        private static Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="recordPrinter">Record printer instance.</param>
        /// <param name="validator">Input validator instance.</param>
        public FindCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, Action<IEnumerable<FileCabinetRecord>> recordPrinter, InputValidator validator)
        {
            service = fileCabinetService;
            FindCommandHandler.recordPrinter = recordPrinter;
            InputValidator = validator;
        }

        /// <summary>
        /// Handles the 'find' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(FindCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                ParceParameters(properties, commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void ShowByName(string propertyName, string value)
        {
            string trimmedValue = value.Trim();
            Tuple<bool, string> validationResult = InputValidator.ValidateStrings(trimmedValue);
            if (!validationResult.Item1)
            {
                Console.WriteLine(validationResult.Item2);
                Console.WriteLine(HintMessage);
            }
            else
            {
                IEnumerable<FileCabinetRecord> records = GetByNameCollection(propertyName, value);
                ShowRecords(records, propertyName, trimmedValue);
            }
        }

        private static IEnumerable<FileCabinetRecord> GetByNameCollection(string propertyName, string value)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            string propertyInLower = propertyName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            return propertyInLower switch
            {
                FirstNameProperty => service.FindByFirstName(value),
                LastNameProperty => service.FindByLastName(value),
                _ => null,
            };
        }

        private static void ShowByDate(string propertyName, string value)
        {
            DateTime dateOfBirth;
            string trimmedValue = value.Trim();
            bool isParsed = DateTime.TryParse(trimmedValue, out dateOfBirth);
            if (isParsed)
            {
                Tuple<bool, string> validationResult = InputValidator.ValidateDate(dateOfBirth);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"There is no records with {propertyName} '{trimmedValue}'.");
                    Console.WriteLine(validationResult.Item2);
                    return;
                }

                IEnumerable<FileCabinetRecord> records = service.FindByDateOfBirth(dateOfBirth);
                ShowRecords(records, propertyName, value);
            }
            else
            {
                Console.WriteLine($"There is no records with {propertyName} '{trimmedValue}'.");
                Console.WriteLine(HintMessage);
            }
        }

        private static void ShowRecords(IEnumerable<FileCabinetRecord> records, string propertyName, string input)
        {
            if (!records.GetEnumerator().MoveNext())
            {
                Console.WriteLine($"There is no records with {propertyName} '{input}'");
            }
            else
            {
                recordPrinter(records);
            }
        }
    }
}
