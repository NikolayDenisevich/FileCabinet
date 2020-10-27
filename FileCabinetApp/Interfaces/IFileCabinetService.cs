using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Defines metohods for FileCabinetServices.
    /// </summary>
    /// <typeparam name="TRecord">Record type.</typeparam>
    /// <typeparam name="TAruments">Arguments type.</typeparam>
    public interface IFileCabinetService<TRecord, TAruments>
    {
        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="arguments">Record arguments.</param>
        /// <returns>Id-number for new record.</returns>
        public int CreateRecord(TAruments arguments);

        /// <summary>
        /// Edits specific record.
        /// </summary>
        /// <param name="arguments">Record arguments.</param>
        public void EditRecord(TAruments arguments);

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<TRecord> GetRecords(string filters, Func<TRecord, bool> predicate);

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <param name="removedRecordsCount">When this method returns, contains deleted record count.</param>
        /// <returns>Records count.</returns>
        public int GetStat(out int removedRecordsCount);

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot(IEnumerable<TRecord> records);

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes specified record.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId);

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <param name="totalRecordsBeforePurgeCount">When this method returns, contains total records before purge.</param>
        /// <returns>Purged records count.</returns>
        public int Purge(out int totalRecordsBeforePurgeCount);
    }
}
