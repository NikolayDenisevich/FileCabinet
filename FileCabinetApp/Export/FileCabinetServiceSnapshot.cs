using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FileCabinetServiceSnapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private IList<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The FileCabinetRecords collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot(IEnumerable<FileCabinetRecord> records) =>
            this.records = records != null ? new List<FileCabinetRecord>(records) : throw new ArgumentNullException($"{nameof(records)}");

        /// <summary>
        /// Gets records readonly collection.
        /// </summary>
        /// <value>
        /// Records readonly collection.
        /// </value>
        public IReadOnlyCollection<FileCabinetRecord> Records => new ReadOnlyCollection<FileCabinetRecord>(this.records);

        /// <summary>
        /// Saves records to csv.
        /// </summary>
        /// <param name="streamWriter">The streamWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when streamWriter is null.</exception>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            streamWriter = streamWriter ?? throw new ArgumentNullException($"{nameof(streamWriter)}");
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            foreach (var item in this.records)
            {
                csvWriter.Write(item);
            }
        }

        /// <summary>
        /// Saves records to xml.
        /// </summary>
        /// <param name="streamWriter">The streamWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when writer is null.</exception>
        public void SaveToXml(StreamWriter streamWriter)
        {
            streamWriter = streamWriter ?? throw new ArgumentNullException($"{nameof(streamWriter)}");
            using var fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(streamWriter);
            foreach (var item in this.records)
            {
                fileCabinetRecordXmlWriter.Write(item);
            }
        }

        /// <summary>
        /// Loads records from csv file.
        /// </summary>
        /// <param name="streamReader">The StreamReader instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when streamReader is null.</exception>
        public void LoadFromCsv(StreamReader streamReader)
        {
            streamReader = streamReader ?? throw new ArgumentNullException($"{nameof(streamReader)}");
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(streamReader);
            this.records = csvReader.ReadAll();
        }

        /// <summary>
        /// Loads records from xml file.
        /// </summary>
        /// <param name="streamReader">The StreamReader instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when streamReader is null.</exception>
        internal void LoadFromXml(StreamReader streamReader)
        {
            streamReader = streamReader ?? throw new ArgumentNullException($"{nameof(streamReader)}");
            FileCabinetRecordXmlReader xmlReader = new FileCabinetRecordXmlReader(streamReader);
            this.records = xmlReader.ReadAll();
        }
    }
}
