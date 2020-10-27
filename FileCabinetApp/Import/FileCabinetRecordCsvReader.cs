using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FileCabinetRecordCsvReader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">StreamReader instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when reader is null.</exception>
        public FileCabinetRecordCsvReader(StreamReader reader) => this.reader = reader ?? throw new ArgumentNullException($"{nameof(reader)}");

        /// <summary>
        /// Reads all records from csv file.
        /// </summary>
        /// <returns>Records list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var list = new List<FileCabinetRecord>();
            while (this.reader.Peek() > 0)
            {
                string line = this.reader.ReadLine();
                FileCabinetRecord cabinetRecord = new FileCabinetRecord();
                string[] stringRecord = line.Split(',');
                for (int i = 0; i < stringRecord.Length; i++)
                {
                    stringRecord[i] = stringRecord[i].Trim();
                }

                try
                {
                    _ = int.TryParse(stringRecord[0], out int intParseResult);
                    cabinetRecord.Id = intParseResult;
                    cabinetRecord.FirstName = stringRecord[1];
                    cabinetRecord.LastName = stringRecord[2];
                    _ = DateTime.TryParse(stringRecord[3], out DateTime dateParseResult);
                    cabinetRecord.DateOfBirth = dateParseResult;
                    _ = short.TryParse(stringRecord[4], out short shortParseResult);
                    cabinetRecord.ZipCode = shortParseResult;
                    cabinetRecord.City = stringRecord[5];
                    cabinetRecord.Street = stringRecord[6];
                    _ = decimal.TryParse(stringRecord[7], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalParseResult);
                    cabinetRecord.Salary = decimalParseResult;
                    _ = char.TryParse(stringRecord[8].Trim(), out char charParseResult);
                    cabinetRecord.Gender = charParseResult;
                    list.Add(cabinetRecord);
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }

            return list;
        }
    }
}
