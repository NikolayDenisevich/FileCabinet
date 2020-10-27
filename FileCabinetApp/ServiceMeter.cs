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
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException($"{nameof(fileCabinetService)}");
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
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.timer.Restart();
            int result = this.fileCabinetService.CreateRecord(arguments);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.CreateRecord));
            return result;
        }

        /// <summary>
        /// Edits a record in the list. Shows elapsed execution time.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.timer.Restart();
            this.fileCabinetService.EditRecord(arguments);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.EditRecord));
        }

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(string filters, Func<FileCabinetRecord, bool> predicate)
        {
            this.timer.Restart();
            var result = this.fileCabinetService.GetRecords(filters, predicate);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.GetRecords));
            return result;
        }

        /// <summary>
        /// Returns records count. Shows elapsed execution time.
        /// </summary>
        /// <param name="removedRecordsCount">When this method returns, contains deleted record count.</param>
        /// <returns>Records count.</returns>
        public int GetStat(out int removedRecordsCount)
        {
            this.timer.Restart();
            var result = this.fileCabinetService.GetStat(out removedRecordsCount);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.GetStat));
            return result;
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance. Shows elapsed execution time.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IEnumerable<FileCabinetRecord> records)
        {
            records = records ?? throw new ArgumentNullException($"{nameof(records)}");
            this.timer.Restart();
            var result = this.fileCabinetService.MakeSnapshot(records);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.MakeSnapshot));
            return result;
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <param name="totalRecordsBeforePurgeCount">When this method returns, contains total records before purge.</param>
        /// <returns>Purged records count.</returns>
        public int Purge(out int totalRecordsBeforePurgeCount)
        {
            this.timer.Restart();
            var result = this.fileCabinetService.Purge(out totalRecordsBeforePurgeCount);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.Purge));
            return result;
        }

        /// <summary>
        /// Removes specified record. Shows elapsed execution time.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            this.timer.Restart();
            this.fileCabinetService.Remove(recordId);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.Remove));
        }

        /// <summary>
        /// Restrores all the containers after import from file. Shows elapsed execution time.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)}");
            this.timer.Restart();
            var result = this.fileCabinetService.Restore(snapshot);
            this.timer.Stop();
            this.ShowElapsedTime(nameof(this.Restore));
            return result;
        }

        private void ShowElapsedTime(string methodName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string executionResult = new StringBuilder()
                .Append($"{methodName} method execution duration is {this.timer.ElapsedTicks} ticks ")
                .Append($"({this.timer.ElapsedMilliseconds} milliseconds).")
                .ToString();
            Console.WriteLine(executionResult);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
