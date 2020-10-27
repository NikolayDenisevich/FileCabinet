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
        /// Removes specified record.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            FileCabinetRecord record = this.list.Find(r => r.Id == recordId);
            if (record is null)
            {
                throw new ArgumentException($"There is no record #{recordId} in the list.");
            }

            this.list.Remove(record);
            this.RemoveRecordFromDictionary(recordId, record.FirstName, record, this.firstNameDictionary);
            this.RemoveRecordFromDictionary(recordId, record.LastName, record, this.lastNameDictionary);
            this.RemoveRecordFromDictionary(recordId, record.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), record, this.dateOfBirthDictionary);
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null");
            }

            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} is null");
            }

            int validRecordsCount = 0;
            IReadOnlyCollection<FileCabinetRecord> records = snapshot.Records;
            foreach (var newItem in records)
            {
                RecordArguments newItemArguments = new RecordArguments
                {
                    Id = newItem.Id,
                    FirstName = newItem.FirstName,
                    LastName = newItem.LastName,
                    DateOfBirth = newItem.DateOfBirth,
                    ZipCode = newItem.ZipCode,
                    City = newItem.City,
                    Street = newItem.Street,
                    Salary = newItem.Salary,
                    Gender = newItem.Gender,
                };
                try
                {
                    this.validator.ValidateArguments(newItemArguments);
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine($"Record #{newItem.Id} failed validation. {exception.Message}");
                    continue;
                }

                FileCabinetRecord existingRecord = this.list.Find(r => r.Id == newItem.Id);

                if (existingRecord != null)
                {
                    this.EditEntryInAllContainers(newItemArguments, existingRecord);
                }
                else
                {
                    this.AddEntryInAllContainers(newItem);
                }

                validRecordsCount++;
            }

            this.list.Sort((r1, r2) => r1.Id.CompareTo(r2.Id));
            return validRecordsCount;
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
            int lastRecordId = this.GetLastRecordId();
            var record = new FileCabinetRecord
            {
                Id = lastRecordId + 1,
                FirstName = arguments.FirstName,
                LastName = arguments.LastName,
                DateOfBirth = arguments.DateOfBirth,
                ZipCode = arguments.ZipCode,
                City = arguments.City,
                Street = arguments.Street,
                Salary = arguments.Salary,
                Gender = arguments.Gender,
            };

            this.AddEntryInAllContainers(record);

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

            this.EditEntryInAllContainers(arguments, listRecord);
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>A sequence of records containing the name 'firstname'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when firstname is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstname length less than 2 or more than 60.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
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
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
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
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth) => GetRecordsFromDictionary(dateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), this.dateOfBirthDictionary);

        /// <summary>
        /// Returns the collection of all records.
        /// </summary>
        /// <returns>The collection of all records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <returns>Records count.</returns>
        public (int, int) GetStat()
        {
            return (this.list.Count, 0);
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <returns>Item1 is purged items count. Item2 total items before purge.</returns>
        public (int, int) Purge()
        {
            Console.WriteLine("You can use this command only for filesystem storage case.");
            return (-1, -1);
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetRecordsFromDictionary(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            dictionary.TryGetValue(key.ToUpperInvariant(), out valuesList);
            ReadOnlyCollection<FileCabinetRecord> reaOnlyCollection;
            reaOnlyCollection = (valuesList is null) ? new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>()) : new ReadOnlyCollection<FileCabinetRecord>(valuesList);
            return reaOnlyCollection;
        }

        private static void EditRecordInDictionary(string oldName, string newName, int id, FileCabinetRecord newRecord, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            if (!oldName.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
            {
                dictionary.TryGetValue(oldName.ToUpperInvariant(), out valuesList);
                FileCabinetRecord oldDictionaryRecord = valuesList.Find(i => i.Id == id);
                valuesList.Remove(oldDictionaryRecord);
                if (valuesList.Count is 0)
                {
                    dictionary.Remove(oldName.ToUpperInvariant(), out valuesList);
                }

                if (dictionary.TryGetValue(newName.ToUpperInvariant(), out valuesList))
                {
                    valuesList.Add(newRecord);
                }
                else
                {
                    valuesList = new List<FileCabinetRecord>();
                    valuesList.Add(newRecord);
                    dictionary.Add(newName.ToUpperInvariant(), valuesList);
                }
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

        private void AddEntryInAllContainers(FileCabinetRecord record)
        {
            this.list.Add(record);
#pragma warning disable CA1062 // Validate arguments of public methods
            AddRecordToDictionary(record.FirstName, record, this.firstNameDictionary);
            AddRecordToDictionary(record.LastName, record, this.lastNameDictionary);
#pragma warning restore CA1062 // Validate arguments of public methods
            AddRecordToDictionary(record.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), record, this.dateOfBirthDictionary);
        }

        private void EditEntryInAllContainers(RecordArguments newArguments, FileCabinetRecord existingRecord)
        {
            string oldFirstName = existingRecord.FirstName;
            string oldLastName = existingRecord.LastName;
            string oldDateOfBirth = existingRecord.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo);
            existingRecord.FirstName = newArguments.FirstName;
            existingRecord.LastName = newArguments.LastName;
            existingRecord.DateOfBirth = newArguments.DateOfBirth;
            existingRecord.ZipCode = newArguments.ZipCode;
            existingRecord.City = newArguments.City;
            existingRecord.Street = newArguments.Street;
            existingRecord.Salary = newArguments.Salary;
            existingRecord.Gender = newArguments.Gender;

#pragma warning disable CA1062 // Validate arguments of public methods
            EditRecordInDictionary(oldFirstName.ToUpperInvariant(), newArguments.FirstName.ToUpperInvariant(), newArguments.Id, existingRecord, this.firstNameDictionary);
            EditRecordInDictionary(oldLastName.ToUpperInvariant(), newArguments.LastName.ToUpperInvariant(), newArguments.Id, existingRecord, this.lastNameDictionary);
#pragma warning restore CA1062 // Validate arguments of public methods
            EditRecordInDictionary(oldDateOfBirth.ToUpperInvariant(), newArguments.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo).ToUpperInvariant(), newArguments.Id, existingRecord, this.dateOfBirthDictionary);
        }

        private int GetLastRecordId()
        {
            return this.list.Count > 0 ? this.list[this.list.Count - 1].Id : this.list.Count;
        }

        private void RemoveRecordFromDictionary(int recordId, string keyName, FileCabinetRecord recordToRemove, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            _ = dictionary.TryGetValue(keyName.ToUpperInvariant(), out valuesList);
            valuesList.Remove(recordToRemove);
            if (valuesList.Count is 0)
            {
                _ = dictionary.Remove(keyName.ToUpperInvariant(), out _);
            }
        }
    }
}
