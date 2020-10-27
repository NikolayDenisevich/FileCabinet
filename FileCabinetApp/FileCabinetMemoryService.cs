using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record memory service.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService<FileCabinetRecord, RecordArguments>
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator<RecordArguments> validator;
        private readonly Dictionary<string, List<FileCabinetRecord>> cache = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The arguments record validator.</param>
        /// <exception cref="ArgumentNullException">Thrown when validator is null.</exception>
        public FileCabinetMemoryService(IRecordValidator<RecordArguments> validator) =>
            this.validator = validator ?? throw new ArgumentNullException($"{nameof(validator)}");

        /// <summary>
        /// Removes specified record.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        /// <exception cref="ArgumentNullException">Thrown if there is no record #recordId in the list.</exception>
        public void Remove(int recordId)
        {
            FileCabinetRecord record = this.list.Find(r => r.Id == recordId);
            record = record ?? throw new ArgumentException($"There is no record #{recordId} in the list.");
            this.list.Remove(record);
            this.cache.Clear();
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IEnumerable<FileCabinetRecord> records) =>
            new FileCabinetServiceSnapshot(records ?? throw new ArgumentNullException($"{nameof(records)}"));

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <returns>Restored records count.</returns>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)}");
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
                    this.EditRecord(newItemArguments, existingRecord);
                }
                else
                {
                    this.list.Add(newItem);
                }

                validRecordsCount++;
            }

            if (validRecordsCount > 0)
            {
                this.cache.Clear();
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
        public int CreateRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.validator.ValidateArguments(arguments);
            int recordId = arguments.Id;
            if (arguments.Id is 0)
            {
                recordId = this.GetLastRecordId();
                recordId++;
            }

            var record = new FileCabinetRecord
            {
                Id = recordId,
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
            this.cache.Clear();
            return record.Id;
        }

        /// <summary>
        /// Edits a record in the list.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.validator.ValidateArguments(arguments);
            FileCabinetRecord listRecord = this.list.Find(i => i.Id == arguments.Id);
            listRecord = listRecord ?? throw new ArgumentException($"There is no record #{arguments.Id} in the list.");
            this.EditRecord(arguments, listRecord);
            this.cache.Clear();
        }

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(string filters, Func<FileCabinetRecord, bool> predicate)
        {
            IEnumerable<FileCabinetRecord> collectionToReturn;
            if (string.IsNullOrWhiteSpace(filters) && predicate is null)
            {
                collectionToReturn = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            }
            else
            {
                var tokens = filters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var builder = new StringBuilder();
                foreach (var item in tokens)
                {
                    builder.Append(item);
                }

                string key = builder.ToString();
                if (this.cache.ContainsKey(key))
                {
                    collectionToReturn = this.cache[key];
                }
                else
                {
                    collectionToReturn = this.list.Where(predicate);
                    this.cache.Add(key, collectionToReturn.ToList());
                }
            }

            return collectionToReturn;
        }

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <param name="removedRecordsCount">When this method returns, contains deleted record count.</param>
        /// <returns>Records count.</returns>
        public int GetStat(out int removedRecordsCount)
        {
            removedRecordsCount = 0;
            return this.list.Count;
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <param name="totalRecordsBeforePurgeCount">When this method returns, contains total records before purge.</param>
        /// <returns>Purged records count.</returns>
        public int Purge(out int totalRecordsBeforePurgeCount)
        {
            Console.WriteLine("You can use this command only for filesystem storage case.");
            totalRecordsBeforePurgeCount = -1;
            return totalRecordsBeforePurgeCount;
        }

        private void EditRecord(RecordArguments newArguments, FileCabinetRecord existingRecord)
        {
            existingRecord.FirstName = newArguments.FirstName;
            existingRecord.LastName = newArguments.LastName;
            existingRecord.DateOfBirth = newArguments.DateOfBirth;
            existingRecord.ZipCode = newArguments.ZipCode;
            existingRecord.City = newArguments.City;
            existingRecord.Street = newArguments.Street;
            existingRecord.Salary = newArguments.Salary;
            existingRecord.Gender = newArguments.Gender;
        }

        private int GetLastRecordId()
        {
            return this.list.Count > 0 ? this.list[^1].Id : this.list.Count;
        }
    }
}
