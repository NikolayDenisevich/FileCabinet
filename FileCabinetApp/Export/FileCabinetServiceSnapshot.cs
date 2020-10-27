using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FileCabinetServiceSnapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private List<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The FileCabinetRecords collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null");
            }

            this.records = new List<FileCabinetRecord>(records);
        }

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
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            if (streamWriter is null)
            {
                throw new ArgumentNullException($"{nameof(streamWriter)} is null");
            }

            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            foreach (var item in this.records)
            {
                csvWriter.Write(item);
            }
        }

        /// <summary>
        /// Saves records to xml.
        /// </summary>
        /// <param name="writer">The streamWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public void SaveToXml(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException($"{nameof(writer)} is null");
            }

            FileCabinetRecordXmlWriter fileCabinetRecordXmlWriter;
            using (XmlWriter xmlWriter = XmlWriter.Create(writer))
            {
                xmlWriter.WriteStartElement($"{nameof(this.records)}");

                fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);
                foreach (var item in this.records)
                {
                    fileCabinetRecordXmlWriter.Write(item);
                }

                xmlWriter.WriteEndDocument();
            }
        }

        /// <summary>
        /// Loads records from csv file.
        /// </summary>
        /// <param name="streamReader">The StreamReader instance.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(streamReader);
            var list = csvReader.ReadAll();
            this.records = list as List<FileCabinetRecord>;
        }

        /// <summary>
        /// Loads records from xml file.
        /// </summary>
        /// <param name="streamReader">The StreamReader instance.</param>
        internal void LoadFromXml(StreamReader streamReader)
        {
            FileCabinetRecordXmlReader xmlReader = new FileCabinetRecordXmlReader(streamReader);
            var list = xmlReader.ReadAll();
            this.records = list as List<FileCabinetRecord>;
        }
    }
}
