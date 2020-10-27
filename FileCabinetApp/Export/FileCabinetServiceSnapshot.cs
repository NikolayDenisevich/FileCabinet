using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FileCabinetServiceSnapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly ReadOnlyCollection<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The FileCabinetRecords collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null");
            }

            this.records = records;
        }

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

            string topic = $"Id, FirstName, LastName, Date of Birth, Zip code, City, Street, Salary, Gender";
            streamWriter.WriteLine(topic);

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
    }
}
