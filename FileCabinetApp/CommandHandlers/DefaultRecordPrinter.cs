using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides records print method.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter<FileCabinetRecord>
    {
        /// <summary>
        /// Defines default records print method.
        /// </summary>
        /// <param name="records">Сollection of records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null.");
            }

            foreach (var item in records)
            {
                Console.WriteLine(item);
            }
        }
    }
}
