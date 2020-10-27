using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FileCabinetRecordXmlReader.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">StreamReader instance.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all records from xml file.
        /// </summary>
        /// <returns>Records list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var list = new List<FileCabinetRecord>();

            XmlHelper xmlData;
            XmlSerializer serializer = new XmlSerializer(typeof(XmlHelper), string.Empty);
#pragma warning disable CA5369 // Use XmlReader For Deserialize
            xmlData = (XmlHelper)serializer.Deserialize(this.reader);
#pragma warning restore CA5369 // Use XmlReader For Deserialize
            foreach (var item in xmlData.Records)
            {
                FileCabinetRecord record = new FileCabinetRecord
                {
                    Id = item.Id,
                    FirstName = item.Name.FirstName,
                    LastName = item.Name.LastName,
                    DateOfBirth = item.DateOfBirth,
                    ZipCode = item.Address.ZipCode,
                    City = item.Address.City,
                    Street = item.Address.Street,
                    Salary = item.Salary,
                    Gender = item.Gender,
                };

                list.Add(record);
            }

            return list;
        }
    }
}
