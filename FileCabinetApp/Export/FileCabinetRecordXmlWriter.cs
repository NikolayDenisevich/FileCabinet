using System;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes the FileCabinetRecordXmlWriter object.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">XmlWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when textWriter is null.</exception>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            if (xmlWriter is null)
            {
                throw new ArgumentNullException($"{nameof(xmlWriter)} is null");
            }

            this.xmlWriter = xmlWriter;
        }

        /// <summary>
        /// Writes the record to file.
        /// </summary>
        /// <param name="record">The filecabinet record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            this.xmlWriter.WriteStartElement($"record");
            {
                this.xmlWriter.WriteAttributeString($"{nameof(record.Id)}", $"{record.Id}");
                {
                    this.xmlWriter.WriteStartElement($"name");
                    {
                        this.xmlWriter.WriteAttributeString($"{nameof(record.FirstName)}", $"{record.FirstName}");
                        this.xmlWriter.WriteAttributeString($"{nameof(record.LastName)}", $"{record.LastName}");
                    }

                    this.xmlWriter.WriteEndElement();
                }

                {
                    this.xmlWriter.WriteStartElement($"{nameof(record.DateOfBirth)}");
                    this.xmlWriter.WriteValue($"{record.DateOfBirth.ToString("dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo)}");
                    this.xmlWriter.WriteEndElement();
                }

                {
                    this.xmlWriter.WriteStartElement($"address");
                    {
                        this.xmlWriter.WriteAttributeString($"{nameof(record.ZipCode)}", $"{record.ZipCode}");
                        this.xmlWriter.WriteAttributeString($"{nameof(record.City)}", $"{record.City}");
                        this.xmlWriter.WriteAttributeString($"{nameof(record.Street)}", $"{record.Street}");
                    }

                    this.xmlWriter.WriteEndElement();
                }

                {
                    this.xmlWriter.WriteStartElement($"{nameof(record.Salary)}");
                    this.xmlWriter.WriteValue($"{record.Salary}");
                    this.xmlWriter.WriteEndElement();
                }

                {
                    this.xmlWriter.WriteStartElement($"{nameof(record.Gender)}");
                    this.xmlWriter.WriteValue($"{record.Gender}");
                    this.xmlWriter.WriteEndElement();
                }
            }

            this.xmlWriter.WriteEndElement();
        }
    }
}
