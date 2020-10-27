using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record service logger.
    /// </summary>
    public sealed class ServiceLogger : IFileCabinetService<FileCabinetRecord, RecordArguments>, IDisposable
    {
        private const string DefaultFileName = "log.txt";
        private const string DateFormat = "dd/MM/yyyy";
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private static readonly CultureInfo InvatiantCulture = CultureInfo.InvariantCulture;
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
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException($"{nameof(fileCabinetService)}");
            this.writer = new StreamWriter(this.filePath, true);
            this.writer.WriteLine($"{DateTime.Now.ToString(DateFormat, InvatiantCulture)}=============== Start logging ===================");
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
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.LogMethodCall(arguments, nameof(this.CreateRecord));
            int result = this.fileCabinetService.CreateRecord(arguments);
            this.LogMethodResult(nameof(this.CreateRecord), result.ToString(InvatiantCulture));
            return result;
        }

        /// <summary>
        /// Edits a record in the list. Writes in file method calling parameters.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.LogMethodCall(arguments, nameof(this.EditRecord));
            this.fileCabinetService.EditRecord(arguments);
            this.LogMethodResult(nameof(this.EditRecord));
        }

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(string filters, Func<FileCabinetRecord, bool> predicate)
        {
            this.LogMethodCall(nameof(this.GetRecords));
            var result = this.fileCabinetService.GetRecords(filters, predicate);
            this.LogMethodResult(nameof(this.GetRecords), result.ToString());
            return result;
        }

        /// <summary>
        /// Returns records count. Writes in file method calling parameters.
        /// </summary>
        /// <param name="removedRecordsCount">When this method returns, contains deleted record count.</param>
        /// <returns>Records count.</returns>
        public int GetStat(out int removedRecordsCount)
        {
            this.LogMethodCall(nameof(this.GetStat));
            var result = this.fileCabinetService.GetStat(out removedRecordsCount);
            this.LogMethodResult(
                nameof(this.GetStat),
                result.ToString(InvatiantCulture),
                nameof(removedRecordsCount),
                removedRecordsCount.ToString(InvatiantCulture));
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
            this.LogMethodCall(records.ToString(), nameof(records), nameof(this.MakeSnapshot));
            var result = this.fileCabinetService.MakeSnapshot(records);
            this.LogMethodResult(nameof(this.MakeSnapshot), result.ToString());
            return result;
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <param name="totalRecordsBeforePurgeCount">When this method returns, contains total records before purge.</param>
        /// <returns>Purged records count.</returns>
        public int Purge(out int totalRecordsBeforePurgeCount)
        {
            this.LogMethodCall(nameof(this.Purge));
            var result = this.fileCabinetService.Purge(out totalRecordsBeforePurgeCount);
            this.LogMethodResult(
                nameof(this.Purge),
                result.ToString(InvatiantCulture),
                nameof(totalRecordsBeforePurgeCount),
                totalRecordsBeforePurgeCount.ToString(InvatiantCulture));
            return result;
        }

        /// <summary>
        /// Removes specified record. Writes in file method calling parameters.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            this.LogMethodCall(recordId.ToString(InvatiantCulture), nameof(recordId), nameof(this.Remove));
            this.fileCabinetService.Remove(recordId);
            this.LogMethodResult(nameof(this.Remove));
        }

        /// <summary>
        /// Restrores all the containers after import from file. Writes in file method calling parameters.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)}");
            this.LogMethodCall(snapshot.ToString(), nameof(snapshot), nameof(this.Restore));
            var result = this.fileCabinetService.Restore(snapshot);
            this.LogMethodResult(nameof(this.Restore), result.ToString(InvatiantCulture));
            return result;
        }

        /// <summary>
        /// Closes stream and writes all buffer deta in the file.
        /// </summary>
        public void Dispose()
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(DateFormat, InvatiantCulture)}=============== End logging ===================");
            this.writer.Close();
        }

        private void LogMethodCall(RecordArguments arguments, string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss", InvatiantCulture)} - calling {methodName} method " +
                $"whith {nameof(arguments.FirstName)} = '{arguments.FirstName}', {nameof(arguments.LastName)} = {arguments.LastName}', " +
                $"{nameof(arguments.DateOfBirth)} = '{arguments.DateOfBirth.ToString("dd/MMM/yyyy", InvatiantCulture)}'" +
                $"{nameof(arguments.ZipCode)} = '{arguments.ZipCode}', {nameof(arguments.City)} = '{arguments.City}', " +
                $"{nameof(arguments.Street)} = '{arguments.Street}', {nameof(arguments.Salary)} = '{arguments.Salary.ToString(InvatiantCulture)}," +
                $"{nameof(arguments.Gender)} = '{arguments.Gender}'");
        }

        private void LogMethodResult(string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(DateTimeFormat, InvatiantCulture)} {methodName} completed");
            this.writer.Flush();
        }

        private void LogMethodResult(string methodName, string returnedValue)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(DateTimeFormat, InvatiantCulture)} {methodName} returned {returnedValue}");
            this.writer.Flush();
        }

        private void LogMethodResult(string methodName, string returnedValue, string outParameterName, string outParameterValue)
        {
            var result = new StringBuilder()
                .Append($"{DateTime.Now.ToString(DateTimeFormat, InvatiantCulture)} ")
                .Append($"{methodName} returned {returnedValue}, ")
                .Append($"out parameter is '{outParameterName}'= {outParameterValue}")
                .ToString();
            this.writer.WriteLine(result);
            this.writer.Flush();
        }

        private void LogMethodCall(string parameter, string parameterName, string methodName)
        {
            var result = new StringBuilder()
                .Append($"{DateTime.Now.ToString(DateTimeFormat, InvatiantCulture)} - ")
                .Append($"calling {methodName} method ")
                .Append($"whith {parameterName} = '{parameter}'")
                .ToString();
            this.writer.WriteLine(result);
            this.writer.Flush();
        }

        private void LogMethodCall(string methodName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString(DateTimeFormat, InvatiantCulture)} - calling {methodName} void method");
            this.writer.Flush();
        }
    }
}
