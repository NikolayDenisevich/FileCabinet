using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record memory service.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService<FileCabinetRecord, RecordArguments>
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly IRecordValidator<RecordArguments> validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The arguments record validator.</param>
        /// <exception cref="ArgumentNullException">Thrown when validator is null.</exception>
        public FileCabinetMemoryService(IRecordValidator<RecordArguments> validator)
        {
            if (validator is null)
            {
                throw new ArgumentNullException($"{nameof(validator)} is null");
            }

            this.validator = validator;
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public static FileCabinetServiceSnapshot MakeSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null");
            }

            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Сreates a record in the list.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <returns>Record ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when arguments.Firstname, or arguments.Lastname, or arguments.City, or arguments.Street is null.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.Firstname, or arguments.Lsatname, or arguments.City, or arguments.Street trimmed length less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.DateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        /// <exception cref="ArgumentException">For parameter arguments.Gender permissible values are :'m', 'M', 'f', 'F'.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.ZipCode range is 1..9999.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.Salary range is 0..100 000.</exception>
        public int CreateRecord(RecordArguments arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException($"{nameof(arguments)} is null");
            }

            this.validator.ValidateArguments(arguments);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = arguments.FirstName,
                LastName = arguments.LastName,
                DateOfBirth = arguments.DateOfBirth,
                ZipCode = arguments.ZipCode,
                City = arguments.City,
                Street = arguments.Street,
                Salary = arguments.Salary,
                Gender = arguments.Gender,
            };

            this.list.Add(record);
#pragma warning disable CA1062 // Validate arguments of public methods
            AddRecordToDictionary(arguments.FirstName, record, this.firstNameDictionary);
            AddRecordToDictionary(arguments.LastName, record, this.lastNameDictionary);
#pragma warning restore CA1062 // Validate arguments of public methods
            AddRecordToDictionary(arguments.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), record, this.dateOfBirthDictionary);

            return record.Id;
        }

        /// <summary>
        /// Edits a record in the list.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when arguments.Firstname, or arguments.Lastname, or arguments.City, or arguments.Street is null.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.Firstname, or arguments.Lsatname, or arguments.City, or arguments.Street trimmed length less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.DateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        /// <exception cref="ArgumentException">For parameter arguments.Gender permissible values are :'m', 'M', 'f', 'F'.</exception>
        /// <exception cref="ArgumentException">There is no record #{arguments.Id} in the list.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.ZipCode range is 1..9999.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.Salary range is 0..100 000.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException($"{nameof(arguments)} is null");
            }

            this.validator.ValidateArguments(arguments);
            FileCabinetRecord listRecord = this.list.Find(i => i.Id == arguments.Id);
            if (listRecord is null)
            {
                throw new ArgumentException($"There is no record #{arguments.Id} in the list.");
            }

            string oldFirstName = listRecord.FirstName;
            string oldLastName = listRecord.LastName;
            string oldDateOfBirth = listRecord.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo);
            listRecord.FirstName = arguments.FirstName;
            listRecord.LastName = arguments.LastName;
            listRecord.DateOfBirth = arguments.DateOfBirth;
            listRecord.ZipCode = arguments.ZipCode;
            listRecord.City = arguments.City;
            listRecord.Street = arguments.Street;
            listRecord.Salary = arguments.Salary;
            listRecord.Gender = arguments.Gender;

#pragma warning disable CA1062 // Validate arguments of public methods
            EditRecordInDictionary(oldFirstName, arguments.FirstName, arguments.Id, listRecord, this.firstNameDictionary);
            EditRecordInDictionary(oldLastName, arguments.LastName, arguments.Id, listRecord, this.lastNameDictionary);
#pragma warning restore CA1062 // Validate arguments of public methods
            EditRecordInDictionary(oldDateOfBirth, arguments.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), arguments.Id, listRecord, this.dateOfBirthDictionary);
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>A sequence of records containing the name 'firstname'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when firstname is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstname length less than 2 or more than 60.</exception>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}.Length should be from 2 to 60. {nameof(firstName)} should not consist only of white-spaces characters");
            }

            return GetRecordsFromDictionary(firstName, this.firstNameDictionary);
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.
        /// </summary>
        /// <param name="lastName">Search key.</param>
        /// <returns>A sequence of records containing the name 'lastName'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when lastName length less than 2 or more than 60.</exception>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}.Length should be from 2 to 60. {nameof(lastName)} should not consist only of white-spaces characters");
            }

            return GetRecordsFromDictionary(lastName, this.lastNameDictionary);
        }

        /// <summary>
        /// Returns a sequence of records containing the date 'dateOfBirth'.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>A sequence of records containing the date 'dateOfBirth'.</returns>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth) => GetRecordsFromDictionary(dateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), this.dateOfBirthDictionary);

        /// <summary>
        /// Returns the collection of all records.
        /// </summary>
        /// <returns>The collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <returns>Records count.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetRecordsFromDictionary(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            dictionary.TryGetValue(key.ToUpperInvariant(), out valuesList);
            ReadOnlyCollection<FileCabinetRecord> reaOnlyCollection;
            reaOnlyCollection = (valuesList is null) ? new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>()) : new ReadOnlyCollection<FileCabinetRecord>(valuesList);
            return reaOnlyCollection;
        }

        private static void EditRecordInDictionary(string oldName, string newName, int id, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            if (oldName.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
            {
                dictionary.TryGetValue(oldName.ToUpperInvariant(), out valuesList);
                FileCabinetRecord dictionaryRecord = valuesList.Find(i => i.Id == id);
                dictionaryRecord = record;
            }
            else
            {
                dictionary.TryGetValue(oldName.ToUpperInvariant(), out valuesList);
                FileCabinetRecord oldDictionaryRecord = valuesList.Find(i => i.Id == id);
                valuesList.Remove(oldDictionaryRecord);
            }

            if (dictionary.TryGetValue(newName.ToUpperInvariant(), out valuesList))
            {
                valuesList.Add(record);
            }
            else
            {
                valuesList = new List<FileCabinetRecord>();
                valuesList.Add(record);
                dictionary.Add(newName.ToUpperInvariant(), valuesList);
            }
        }

        private static void AddRecordToDictionary(string name, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            if (dictionary.TryGetValue(name.ToUpperInvariant(), out valuesList))
            {
                valuesList.Add(record);
            }
            else
            {
                valuesList = new List<FileCabinetRecord>();
                valuesList.Add(record);
                dictionary.Add(name.ToUpperInvariant(), valuesList);
            }
        }
    }
}
