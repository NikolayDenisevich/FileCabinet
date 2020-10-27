using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes the FileCabinetRecordCsvWriter writer object.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Textwriter instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when textWriter is null.</exception>
        public FileCabinetRecordCsvWriter(TextWriter textWriter) =>
            this.textWriter = textWriter ?? throw new ArgumentNullException($"{nameof(textWriter)}");

        /// <summary>
        /// Writes the record to file.
        /// </summary>
        /// <param name="record">The filecabinet record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            record = record ?? throw new ArgumentNullException($"{nameof(record)}");
            this.textWriter.WriteLine(record.ToString());
        }
    }
}
