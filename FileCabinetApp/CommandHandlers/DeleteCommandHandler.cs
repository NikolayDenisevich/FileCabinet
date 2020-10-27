using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides the 'delete' command handler for FileCabinetApp.
    /// </summary>
    internal class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string DeleteCommand = "delete";
        private const string WhereLiteral = "where";

        private static Tuple<string, Action<string, string>>[] properties = new Tuple<string, Action<string, string>>[]
        {
            new Tuple<string, Action<string, string>>(IdName, DeleteById),
            new Tuple<string, Action<string, string>>(FirstNameName, DeleteByName),
            new Tuple<string, Action<string, string>>(LastNameName, DeleteByName),
            new Tuple<string, Action<string, string>>(DateOfBirthName, DeleteByDate),
            new Tuple<string, Action<string, string>>(ZipCodeName, DeleteByZipCode),
            new Tuple<string, Action<string, string>>(CityName, DeleteByName),
            new Tuple<string, Action<string, string>>(StreetName, DeleteByName),
            new Tuple<string, Action<string, string>>(SalaryName, DeleteBySalary),
            new Tuple<string, Action<string, string>>(GenderName, DeleteByGender),
        };

        private static IFileCabinetService<FileCabinetRecord, RecordArguments> service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabietService instance.</param>
        /// <param name="validator">InputValidator instance.</param>
        public DeleteCommandHandler(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService, InputValidator validator)
        {
            service = fileCabinetService;
            InputValidator = validator;
        }

        /// <summary>
        /// Handles the 'delete' command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command.Equals(DeleteCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                Delete(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Delete(string parameters)
        {
            const char EqualChar = '=';
            if (!IsCorrectWhereParameter(parameters))
            {
                return;
            }
            else
            {
                parameters = parameters.Replace(WhereLiteral, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
                ParceParameters(properties, parameters, EqualChar);
            }
        }

        private static bool IsCorrectWhereParameter(string parameters)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(parameters);
            if (isEmpty)
            {
                PrintParametrizedCommandHint();
            }

            bool isCorrect = !isEmpty && parameters.Contains(WhereLiteral, StringComparison.InvariantCultureIgnoreCase);
            if (!isCorrect)
            {
                PrintIncorrectSyntax(parameters);
            }

            return isCorrect;
        }

        private static void DeleteById(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, IntConverter, InputValidator.ValidateInt, out int id);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, id);
            }
        }

        private static void DeleteBySalary(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, DecimalConverter, InputValidator.ValidateDecimal, out decimal salary);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, salary);
            }
        }

        private static void DeleteByZipCode(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, ShortConverter, InputValidator.ValidateShort, out short zipCode);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, zipCode);
            }
        }

        private static void DeleteByGender(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, CharConverter, InputValidator.ValidateChar, out char gender);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, gender);
            }
        }

        private static void DeleteByDate(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, DatesConverter, InputValidator.ValidateDate, out DateTime date);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, date);
            }
        }

        private static void DeleteByValue<T>(string propertyName, T value)
        {
            var recordsToDelete = GetByValueCollection(propertyName, value).ToArray();
            if (!recordsToDelete.Any())
            {
                PrintRecordDoesntExists(propertyName, value.ToString());
            }
            else
            {
                foreach (var item in recordsToDelete)
                {
                    service.Remove(item.Id);
                }

                PrintOperationResult(recordsToDelete, DeleteCommand);
            }
        }

        private static void DeleteByName(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PrintParametrizedCommandHint();
                return;
            }

            bool isValid = TryCheckInput(value, StringsConverter, InputValidator.ValidateStrings, out string name);
            if (!isValid)
            {
                return;
            }
            else
            {
                DeleteByValue(propertyName, name);
            }
        }

        private static IEnumerable<FileCabinetRecord> GetByValueCollection<T>(string propertyName, T value)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            string propertyInLower = propertyName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            return propertyInLower switch
            {
                IdName => service.GetRecords().Where(r => r.Id == Convert.ToInt32(value, CultureInfo.InvariantCulture)).Take(1),
                FirstNameName => service.FindByFirstName(value.ToString()),
                LastNameName => service.FindByLastName(value.ToString()),
                CityName => service.GetRecords().Where(r => r.City.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                StreetName => service.GetRecords().Where(r => r.Street.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                ZipCodeName => service.GetRecords().Where(r => r.ZipCode == Convert.ToInt16(value, CultureInfo.InvariantCulture)),
                SalaryName => service.GetRecords().Where(r => r.Salary == Convert.ToDecimal(value, CultureInfo.InvariantCulture)),
                GenderName => service.GetRecords().Where(r => r.Gender == Convert.ToChar(value, CultureInfo.InvariantCulture)),
                DateOfBirthName => service.FindByDateOfBirth(Convert.ToDateTime(value, CultureInfo.InvariantCulture)),
                _ => null,
            };
        }

        private static void PrintRecordDoesntExists(string propertyName, string value)
        {
            Console.WriteLine($"There is no records with {propertyName} '{value}'");
        }
    }
}
