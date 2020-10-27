using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        /// <returns>Readonly records collection.</returns>
        public ReadOnlyCollection<TRecord> GetRecords();

        /// <summary>
        /// Returns records quantity.
        /// </summary>
        /// <returns>records quantity.</returns>
        public int GetStat();
    }
}
