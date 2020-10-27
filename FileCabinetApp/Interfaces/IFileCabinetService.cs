using System;
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
        public IReadOnlyCollection<TRecord> GetRecords();

        /// <summary>
        /// Returns records quantity.
        /// </summary>
        /// <returns>records quantity.</returns>
        public int GetStat();

        /// <summary>
        /// Returns records collection that contains records with the specified firstname.
        /// </summary>
        /// <param name="firstName">The specified firstname.</param>
        /// <returns>Records collection.</returns>
        public IReadOnlyCollection<TRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Returns records collection that contains records with the specified lastName.
        /// </summary>
        /// <param name="lastName">The specified lastName.</param>
        /// <returns>Records collection.</returns>
        public IReadOnlyCollection<TRecord> FindByLastName(string lastName);

        /// <summary>
        /// Returns records collection that contains records with the specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The specified date of birth.</param>
        /// <returns>Records collection.</returns>
        public IReadOnlyCollection<TRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot(IReadOnlyCollection<TRecord> records);

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot);
    }
}
