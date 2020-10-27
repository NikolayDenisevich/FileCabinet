using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes the FileCabinetRecordXmlWriter object.
    /// </summary>
    public class FileCabinetRecordXmlWriter : IDisposable
    {
        private readonly XmlWriter xmlWriter;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="streamWriter">streamWriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when streamWriter is null.</exception>
        public FileCabinetRecordXmlWriter(StreamWriter streamWriter)
        {
            streamWriter = streamWriter ?? throw new ArgumentNullException($"{nameof(streamWriter)}");
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Encoding = Encoding.Unicode;
            settings.CloseOutput = false;
            settings.WriteEndDocumentOnClose = true;
            settings.Indent = true;
            this.xmlWriter = XmlWriter.Create(streamWriter, settings);
            this.xmlWriter.WriteStartDocument();
            this.xmlWriter.WriteStartElement($"records");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        ~FileCabinetRecordXmlWriter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Writes the record to file.
        /// </summary>
        /// <param name="record">The filecabinet record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            record = record ?? throw new ArgumentNullException($"{nameof(record)}");

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

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Frees resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.xmlWriter.WriteEndDocument();
                this.xmlWriter.Dispose();
            }

            this.disposed = true;
        }
    }
}
