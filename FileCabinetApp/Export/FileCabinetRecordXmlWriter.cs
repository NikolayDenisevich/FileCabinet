using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes the FileCabinetRecordXmlWriter object.
    /// </summary>
    public sealed class FileCabinetRecordXmlWriter : IDisposable
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="streamWriter">streamWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when textWriter is null.</exception>
        public FileCabinetRecordXmlWriter(StreamWriter streamWriter)
        {
            streamWriter = streamWriter ?? throw new ArgumentNullException($"{nameof(streamWriter)} is null");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Encoding = Encoding.Unicode;
            settings.CloseOutput = false;
            settings.WriteEndDocumentOnClose = true;
            settings.Indent = true;
            Console.WriteLine(settings.OutputMethod);
            this.xmlWriter = XmlWriter.Create(streamWriter, settings);
            this.xmlWriter.WriteStartDocument();
            this.xmlWriter.WriteStartElement($"records");
        }

        /// <summary>
        /// Writes EndDocument.
        /// </summary>
        public void Dispose()
        {
            this.xmlWriter.WriteEndDocument();
            this.xmlWriter.Close();
        }

        /// <summary>
        /// Writes the record to file.
        /// </summary>
        /// <param name="record">The filecabinet record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            record = record ?? throw new ArgumentNullException($"{nameof(record)} is null");

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
                    this.xmlWriter.WriteValue($"{record.DateOfBirth.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo)}");
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
                    this.xmlWriter.WriteValue($"{record.Salary.ToString(CultureInfo.InvariantCulture)}");
                    this.xmlWriter.WriteEndElement();
                }

                {
                    this.xmlWriter.WriteStartElement($"{nameof(record.Gender)}");
                    this.xmlWriter.WriteValue($"{record.Gender.ToString(CultureInfo.InvariantCulture)}");
                    this.xmlWriter.WriteEndElement();
                }
            }

            this.xmlWriter.WriteEndElement();
        }
    }
}
