using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record service logger.
    /// </summary>
    public sealed class ServiceLogger : IFileCabinetService<FileCabinetRecord, RecordArguments>, IDisposable
    {
        private const string DefaultFileName = "log.txt";
        private const string CreateRecordName = "CreateRecord";
        private const string EditRecordName = "EditRecord";
        private const string GetRecordsName = "GetRecords";
        private const string GetStatName = "GetStat";
        private const string MakeSnapshotName = "MakeSnapshot";
        private const string PurgeName = "Purge";
        private const string RemoveName = "Remove";
        private const string RestoreName = "Restore";
        private static string dateFormat = "dd/MM/yyyy";
        private static string dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private static CultureInfo invatiantCulture = CultureInfo.InvariantCulture;
        private readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), DefaultFileName);
        private readonly IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService;
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The fileCabinetService instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when fileCabinetService is null.</exception>
        public ServiceLogger(IFileCabinetService<FileCabinetRecord, RecordArguments> fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException($"{nameof(fileCabinetService)} is null");
            this.writer = new StreamWriter(this.filePath, true);
            this.writer.WriteLine($"{DateTime.Now.ToString(dateFormat, invatiantCulture)}=============== Start logging ===================");
            this.writer.Flush();
        }

        /// <summary>
        /// Сreates a record in the list. Writes in file method calling parameters.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <returns>Record ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public int CreateRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)} is null");
            this.LogMethodCall(arguments, CreateRecordName);
            int result = this.fileCabinetService.CreateRecord(arguments);
            this.LogMethodResult(CreateRecordName, result.ToString(invatiantCulture));
            return result;
        }

        /// <summary>
        /// Edits a record in the list. Writes in file method calling parameters.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)} is null");
            this.LogMethodCall(arguments, EditRecordName);
            this.fileCabinetService.EditRecord(arguments);
            this.LogMethodResult(EditRecordName);
        }

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(string filters, Func<FileCabinetRecord, bool> predicate)
        {
            this.LogMethodCall(GetRecordsName);
            var result = this.fileCabinetService.GetRecords(filters, predicate);
            this.LogMethodResult(GetRecordsName, result.ToString());
            return result;
        }

        /// <summary>
        /// Returns records count. Writes in file method calling parameters.
        /// </summary>
        /// <returns>Records count.</returns>
        public (int, int) GetStat()
        {
            this.LogMethodCall(GetStatName);
            var result = this.fileCabinetService.GetStat();
            this.LogMethodResult(GetStatName, result.ToString());
            return result;
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance. Writes in file method calling parameters.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IEnumerable<FileCabinetRecord> records)
        {
            records = records ?? throw new ArgumentNullException($"{nameof(records)}");
            this.LogMethodCall(records.ToString(), nameof(records), MakeSnapshotName);
            var result = this.fileCabinetService.MakeSnapshot(records);
            this.LogMethodResult(MakeSnapshotName, result.ToString());
            return result;
        }

        /// <summary>
        /// Pugres the data file. Writes in file method calling parameters.
        /// </summary>
        /// <returns>Item1 is purged items count. Item2 total items before purge.</returns>
        public (int, int) Purge()
        {
            this.LogMethodCall(PurgeName);
            var result = this.fileCabinetService.Purge();
            this.LogMethodResult(PurgeName, result.ToString());
            return result;
        }

        /// <summary>
        /// Removes specified record. Writes in file method calling parameters.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            this.LogMethodCall(RemoveName);
            this.fileCabinetService.Remove(recordId);
            this.LogMethodResult(RemoveName);
        }

        /// <summary>
        /// Restrores all the containers after import from file. Writes in file method calling parameters.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)} is null");
            this.LogMethodCall(RestoreName, nameof(snapshot), RestoreName);
            var result = this.fileCabinetService.Restore(snapshot);
            this.LogMethodResult(RestoreName, result.ToString(invatiantCulture));
            return result;
        }

        /// <summary>
        /// Closes stream and writes all buffer deta in the file.
        /// </summary>
        public void Dispose()
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(dateFormat, invatiantCulture)}=============== End logging ===================");
            this.writer.Close();
        }

        private void LogMethodCall(RecordArguments arguments, string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss", invatiantCulture)} - calling {methodName} method " +
                $"whith {nameof(arguments.FirstName)} = '{arguments.FirstName}', {nameof(arguments.LastName)} = {arguments.LastName}', " +
                $"{nameof(arguments.DateOfBirth)} = '{arguments.DateOfBirth.ToString("dd/MMM/yyyy", invatiantCulture)}'" +
                $"{nameof(arguments.ZipCode)} = '{arguments.ZipCode}', {nameof(arguments.City)} = '{arguments.City}', " +
                $"{nameof(arguments.Street)} = '{arguments.Street}', {nameof(arguments.Salary)} = '{arguments.Salary.ToString(invatiantCulture)}," +
                $"{nameof(arguments.Gender)} = '{arguments.Gender}'");
        }

        private void LogMethodResult(string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(dateTimeFormat, invatiantCulture)} {methodName} completed");
            this.writer.Flush();
        }

        private void LogMethodResult(string methodName, string returnedValue)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(dateTimeFormat, invatiantCulture)} {methodName} returned {returnedValue}");
            this.writer.Flush();
        }

        private void LogMethodCall(string parameter, string parameterName, string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(dateTimeFormat, invatiantCulture)} - calling {methodName} method " +
                $"whith {parameterName} = '{parameter}'");
            this.writer.Flush();
        }

        private void LogMethodCall(string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(dateTimeFormat, invatiantCulture)} - calling {methodName} void method");
            this.writer.Flush();
        }
    }
}
