using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes the FileCabinetRecordCsvWriter writer object.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Textwriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when textWriter is null.</exception>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            if (textWriter is null)
            {
                throw new ArgumentNullException($"{nameof(textWriter)} is null");
            }

            this.textWriter = textWriter;
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

            this.textWriter.WriteLine(record.ToString());
        }
    }
}
