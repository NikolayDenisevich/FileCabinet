using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record service meter.
    /// </summary>
    public class ServiceMeter : IFileCabinetService<FileCabinetRecord, RecordArguments>
    {
        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService;
        private readonly Stopwatch timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The fileCabinetService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public ServiceMeter(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException($"{nameof(fileCabinetService)} is null");
            this.timer = new Stopwatch();
        }

        /// <summary>
        /// Сreates a record in the list. Shows elapsed execution time.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <returns>Record ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public int CreateRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)} is null");
            this.timer.Start();
            int result = this.fileCabinetService.CreateRecord(arguments);
            this.timer.Stop();
            this.ShowElapsedTime("CreateRecord");
            return result;
        }

        /// <summary>
        /// Edits a record in the list. Shows elapsed execution time.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            this.timer.Start();
            this.fileCabinetService.EditRecord(arguments);
            this.timer.Stop();
            this.ShowElapsedTime("EditRecord");
        }

        /// <summary>
        /// Returns a sequence of records containing the date 'dateOfBirth'. Shows elapsed execution time.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>A sequence of records containing the date 'dateOfBirth'.</returns>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.timer.Start();
            var result = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            this.timer.Stop();
            this.ShowElapsedTime("FindByDateOfBirth");
            return result;
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'. Shows elapsed execution time.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>A sequence of records containing the name 'firstname'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when firstname is null.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            firstName = firstName ?? throw new ArgumentNullException($"{nameof(firstName)} is null");
            this.timer.Start();
            var result = this.fileCabinetService.FindByFirstName(firstName);
            this.timer.Stop();
            this.ShowElapsedTime("FindByFirstName");
            return result;
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.Shows elapsed execution time.
        /// </summary>
        /// <param name="lastName">Search key.</param>
        /// <returns>A sequence of records containing the name 'lastName'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when lastName is null.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            lastName = lastName ?? throw new ArgumentNullException($"{nameof(lastName)} is null");
            this.timer.Start();
            var result = this.fileCabinetService.FindByLastName(lastName);
            this.timer.Stop();
            this.ShowElapsedTime("FindByLastName");
            return result;
        }

        /// <summary>
        /// Returns the collection of all records. Shows elapsed execution time.
        /// </summary>
        /// <returns>The collection of all records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.timer.Start();
            var result = this.fileCabinetService.GetRecords();
            this.timer.Stop();
            this.ShowElapsedTime("GetRecords");
            return result;
        }

        /// <summary>
        /// Returns records count. Shows elapsed execution time.
        /// </summary>
        /// <returns>Records count.</returns>
        public (int, int) GetStat()
        {
            this.timer.Start();
            var result = this.fileCabinetService.GetStat();
            this.timer.Stop();
            this.ShowElapsedTime("GetStat");
            return result;
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance. Shows elapsed execution time.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            records = records ?? throw new ArgumentNullException($"{nameof(records)} is null");
            this.timer.Start();
            var result = this.fileCabinetService.MakeSnapshot(records);
            this.timer.Stop();
            this.ShowElapsedTime("MakeSnapshot");
            return result;
        }

        /// <summary>
        /// Pugres the data file. Shows elapsed execution time.
        /// </summary>
        /// <returns>Item1 is purged items count. Item2 total items before purge.</returns>
        public (int, int) Purge()
        {
            this.timer.Start();
            var result = this.fileCabinetService.Purge();
            this.timer.Stop();
            this.ShowElapsedTime("Purge");
            return result;
        }

        /// <summary>
        /// Removes specified record. Shows elapsed execution time.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            this.timer.Start();
            this.fileCabinetService.Remove(recordId);
            this.timer.Stop();
            this.ShowElapsedTime("Remove");
        }

        /// <summary>
        /// Restrores all the containers after import from file. Shows elapsed execution time.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)} is null");
            this.timer.Start();
            var result = this.fileCabinetService.Restore(snapshot);
            this.timer.Stop();
            this.ShowElapsedTime("Restore");
            return result;
        }

        private void ShowElapsedTime(string methodName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{methodName} method execution duration is {this.timer.ElapsedTicks} ticks ({this.timer.ElapsedMilliseconds} milliseconds)");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
