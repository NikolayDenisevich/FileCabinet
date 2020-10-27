using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Provides records print method.
    /// </summary>
    /// <typeparam name="T">Record type.</typeparam>
    public interface IRecordPrinter<T>
    {
        /// <summary>
        /// Defines records print method.
        /// </summary>
        /// <param name="records">Сollection of records to print.</param>
        public void Print(IEnumerable<T> records);
    }
}
