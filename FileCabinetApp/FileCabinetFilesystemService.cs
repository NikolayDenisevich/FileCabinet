using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a file cabinet record file system service.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService<FileCabinetRecord, RecordArguments>
    {
        private const int StringValuesLengthInBytes = 120;
        private const int BytesInOneUnicodeSymbol = 2;
        private const int OneRecordFullLengthInBytes = 518;
        private const short DeleteMask = 4;
        private readonly FileStream fileStream;
        private readonly IRecordValidator<RecordArguments> validator;
#pragma warning disable SA1305 // Field names should not use Hungarian notation
        private readonly Dictionary<int, long> idIndexer = new Dictionary<int, long>();
#pragma warning restore SA1305 // Field names should not use Hungarian notation
        private int currentRecordsCount;
        private int maxId;
        private int deletedRecordsCount;
        private bool isRecordReadedFromImportedFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The arguments record validator.</param>
        /// <param name="fileStream">File stream for working with data file.</param>
        public FileCabinetFilesystemService(IRecordValidator<RecordArguments> validator, FileStream fileStream)
        {
            this.validator = validator ?? throw new ArgumentNullException($"{nameof(validator)}");
            this.fileStream = fileStream ?? throw new ArgumentNullException($"{nameof(fileStream)}");
            this.InintializeService();
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IEnumerable<FileCabinetRecord> records)
        {
            records = records ?? throw new ArgumentNullException($"{nameof(records)}");
            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <returns>Item1 is purged items count. Item2 total items before purge.</returns>
        public (int, int) Purge()
        {
            int totalRecordsCount = (int)(this.fileStream.Length / OneRecordFullLengthInBytes);
            (int, int) valueToReturn;
            if (this.deletedRecordsCount > 0)
            {
                var notMarkedRecords = this.GetRecords(null, null);
                valueToReturn = (this.deletedRecordsCount, totalRecordsCount);
                this.ReinitializeServiceThroughWritingRecordsCollection(notMarkedRecords);
            }
            else
            {
                valueToReturn = (-1, -1);
            }

            return valueToReturn;
        }

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            snapshot = snapshot ?? throw new ArgumentNullException($"{nameof(snapshot)}");
            int validRecordsCount = 0;
            IReadOnlyCollection<FileCabinetRecord> records = snapshot.Records;

            foreach (var newItem in records)
            {
                RecordArguments newItemArguments = new RecordArguments
                {
                    Id = newItem.Id,
                    FirstName = newItem.FirstName,
                    LastName = newItem.LastName,
                    DateOfBirth = newItem.DateOfBirth,
                    ZipCode = newItem.ZipCode,
                    City = newItem.City,
                    Street = newItem.Street,
                    Salary = newItem.Salary,
                    Gender = newItem.Gender,
                };

                try
                {
                    this.validator.ValidateArguments(newItemArguments);
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine($"Record #{newItem.Id} failed validation. {exception.Message}");
                    continue;
                }

                long foundRecordPosition = this.FindRecordPosition(newItem.Id);
                if (foundRecordPosition is -1)
                {
                    this.isRecordReadedFromImportedFile = true;
                    this.CreateRecord(newItemArguments);
                }
                else
                {
                    this.EditExistingRecord(newItemArguments, foundRecordPosition);
                }

                validRecordsCount++;
            }

            return validRecordsCount;
        }

        /// <summary>
        /// Сreates a record in the file.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <returns>Record ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when arguments.Firstname, or arguments.Lastname, or arguments.City, or arguments.Street is null.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.Firstname, or arguments.Lsatname, or arguments.City, or arguments.Street trimmed length less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.DateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        /// <exception cref="ArgumentException">For parameter arguments.Gender permissible values are :'m', 'M', 'f', 'F'.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.ZipCode range is 1..9999.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.Salary range is 0..100 000.</exception>
        public int CreateRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            this.validator.ValidateArguments(arguments);
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                binaryWriter.Seek(0, SeekOrigin.End);
                long currentRecordStartPosition = this.fileStream.Position;
                if (!this.isRecordReadedFromImportedFile)
                {
                    arguments.Id = ++this.maxId;
                    this.WriteArguments(arguments, binaryWriter);
                }
                else
                {
                    this.WriteArguments(arguments, binaryWriter);
                    if (arguments.Id > this.maxId)
                    {
                        this.maxId = arguments.Id;
                    }
                }

                this.AddRecordToIdsIndexer(currentRecordStartPosition, arguments.Id, this.idIndexer);
            }

            this.isRecordReadedFromImportedFile = default;
            this.currentRecordsCount++;
            return this.maxId;
        }

        /// <summary>
        /// Edits a record in the file.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        /// <exception cref="ArgumentException">There is no record #{arguments.Id} in the list.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            arguments = arguments ?? throw new ArgumentNullException($"{nameof(arguments)}");
            long foundRecordPosition = this.FindRecordPosition(arguments.Id);
            if (foundRecordPosition is -1)
            {
                throw new ArgumentException($"There is no record #{arguments.Id} in the file.");
            }

            this.EditExistingRecord(arguments, foundRecordPosition);
        }

        /// <summary>
        /// Removes specified record.
        /// </summary>
        /// <param name="recordId">Record Id.</param>
        public void Remove(int recordId)
        {
            long recordPosition = this.FindRecordPosition(recordId);
            if (recordPosition is -1)
            {
                throw new ArgumentException($"There is no record #{recordId} in the file.");
            }

            this.SetRecordStatusDelete(recordPosition);
            this.RemoveFromIndexers(recordPosition);
            this.currentRecordsCount--;
            this.deletedRecordsCount++;
        }

        /// <summary>
        /// Returns records collection.
        /// </summary>
        /// <param name="filters">Filters string representation.</param>
        /// <param name="predicate">Filtering condition.</param>
        /// <returns>Readonly records collection.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords(string filters, Func<FileCabinetRecord, bool> predicate)
        {
            var recordsList = new List<FileCabinetRecord>();
            using BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true);
            foreach (var keyValuePair in this.idIndexer)
            {
                FileCabinetRecord record = this.ReadRecordFromFile(reader, keyValuePair.Value);
                recordsList.Add(record);
            }

            return predicate is null ? recordsList : recordsList.Where(predicate);
        }

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <returns>Records count.</returns>
        public (int, int) GetStat()
        {
            return (this.currentRecordsCount, this.deletedRecordsCount);
        }

        private static void WriteEmptyBytesFromEndOfStringValue(string value, BinaryWriter binaryWriter)
        {
            int bytesToNextRecord = StringValuesLengthInBytes - ((value.Length * BytesInOneUnicodeSymbol) + 1);
            byte[] bytes = new byte[bytesToNextRecord];
            binaryWriter.Write(bytes);
        }

        private static void WriteRecord(FileCabinetRecord record, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((short)0);
            binaryWriter.Write(record.Id);
            binaryWriter.Write(record.FirstName);
            WriteEmptyBytesFromEndOfStringValue(record.FirstName, binaryWriter);
            binaryWriter.Write(record.LastName);
            WriteEmptyBytesFromEndOfStringValue(record.LastName, binaryWriter);
            binaryWriter.Write(record.DateOfBirth.Year);
            binaryWriter.Write(record.DateOfBirth.Month);
            binaryWriter.Write(record.DateOfBirth.Day);
            binaryWriter.Write(record.ZipCode);
            binaryWriter.Write(record.City);
            WriteEmptyBytesFromEndOfStringValue(record.City, binaryWriter);
            binaryWriter.Write(record.Street);
            WriteEmptyBytesFromEndOfStringValue(record.Street, binaryWriter);
            binaryWriter.Write(record.Salary);
            binaryWriter.Write(record.Gender);
        }

        private FileCabinetRecord ReadRecordFromFile(BinaryReader reader, long fileStreamPosition)
        {
            var record = new FileCabinetRecord();
            this.fileStream.Seek(fileStreamPosition, SeekOrigin.Begin);
            reader.ReadInt16();
            record.Id = reader.ReadInt32();
            record.FirstName = reader.ReadString();
            this.MoveStreamToNextPositionAfterString(record.FirstName);
            record.LastName = reader.ReadString();
            this.MoveStreamToNextPositionAfterString(record.LastName);
            int year = reader.ReadInt32();
            int month = reader.ReadInt32();
            int day = reader.ReadInt32();
            record.DateOfBirth = new DateTime(year, month, day);
            short zipCode = reader.ReadInt16();
            record.ZipCode = zipCode;
            record.City = reader.ReadString();
            this.MoveStreamToNextPositionAfterString(record.City);
            record.Street = reader.ReadString();
            this.MoveStreamToNextPositionAfterString(record.Street);
            record.Salary = reader.ReadDecimal();
            record.Gender = reader.ReadChar();
            return record;
        }

        private void WriteArguments(RecordArguments arguments, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((short)0);
            binaryWriter.Write(arguments.Id);
            binaryWriter.Write(arguments.FirstName);
            this.MoveStreamToNextPositionAfterString(arguments.FirstName);
            binaryWriter.Write(arguments.LastName);
            this.MoveStreamToNextPositionAfterString(arguments.LastName);
            binaryWriter.Write(arguments.DateOfBirth.Year);
            binaryWriter.Write(arguments.DateOfBirth.Month);
            binaryWriter.Write(arguments.DateOfBirth.Day);
            binaryWriter.Write(arguments.ZipCode);
            binaryWriter.Write(arguments.City);
            this.MoveStreamToNextPositionAfterString(arguments.City);
            binaryWriter.Write(arguments.Street);
            this.MoveStreamToNextPositionAfterString(arguments.Street);
            binaryWriter.Write(arguments.Salary);
            binaryWriter.Write(arguments.Gender);
        }

        private void MoveStreamToNextPositionAfterString(string value)
        {
            int nextOffset = StringValuesLengthInBytes - ((value.Length * BytesInOneUnicodeSymbol) + 1);
            this.fileStream.Seek(nextOffset, SeekOrigin.Current);
        }

        private long FindRecordPosition(int id)
        {
            bool isFound = this.idIndexer.TryGetValue(id, out long position);
            if (isFound)
            {
                return position;
            }

            return -1;
        }

        private void InintializeService()
        {
            int factor = 0;
            using BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true);
            this.fileStream.Seek(factor, SeekOrigin.Begin);
            while (reader.PeekChar() != -1)
            {
                long currentRecordStartPosition = OneRecordFullLengthInBytes * factor;
                this.fileStream.Seek(currentRecordStartPosition, SeekOrigin.Begin);
                short status = reader.ReadInt16();
                status &= DeleteMask;
                if (status != DeleteMask)
                {
                    this.AddToIndexerId(reader, currentRecordStartPosition);
                    this.currentRecordsCount++;
                }
                else
                {
                    this.deletedRecordsCount++;
                }

                this.fileStream.Seek(OneRecordFullLengthInBytes * ++factor, SeekOrigin.Begin);
            }
        }

        private void ReinitializeServiceThroughWritingRecordsCollection(IEnumerable<FileCabinetRecord> notMarkedRecords)
        {
            this.idIndexer.Clear();
            this.deletedRecordsCount = 0;
            this.currentRecordsCount = 0;

            using (BinaryWriter writer = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                writer.Seek(0, SeekOrigin.Begin);
                foreach (var record in notMarkedRecords)
                {
                    this.currentRecordsCount++;
                    this.AddRecordToIdsIndexer(this.fileStream.Position, record.Id, this.idIndexer);
                    WriteRecord(record, writer);
                }

                this.fileStream.SetLength(OneRecordFullLengthInBytes * notMarkedRecords.Count());
            }
        }

        private void AddToIndexerId(BinaryReader reader, long currentRecordStartPosition)
        {
            this.fileStream.Seek(currentRecordStartPosition, SeekOrigin.Begin);
            var currentRecordValues = this.ReadRecordFirstFourFields(reader);
            this.AddRecordToIdsIndexer(currentRecordStartPosition, currentRecordValues.Id, this.idIndexer);
        }

        private void RemoveFromIndexers(long currentRecordStartPosition)
        {
            using var reader = new BinaryReader(this.fileStream, Encoding.Unicode, true);
            this.fileStream.Seek(currentRecordStartPosition, SeekOrigin.Begin);
            var recordToRemove = this.ReadRecordFirstFourFields(reader);
            this.RemoveRecordFromIdsIndexer(recordToRemove.Id, this.idIndexer);
        }

        private void RemoveRecordFromIdsIndexer(int key, Dictionary<int, long> indexer)
        {
            indexer.Remove(key);
            if (key == this.maxId)
            {
                this.maxId = this.idIndexer.Keys.Max();
            }
        }

        private void AddRecordToIdsIndexer(long currentRecordStartPosition, int currentId, Dictionary<int, long> idsIndexer)
        {
            if (currentId > this.maxId)
            {
                this.maxId = currentId;
            }

            bool isExistKey = idsIndexer.TryGetValue(currentId, out long value);
            if (!isExistKey)
            {
                idsIndexer.Add(currentId, currentRecordStartPosition);
            }
        }

        private void EditExistingRecord(RecordArguments arguments, long recordPosition)
        {
            FileCabinetRecord oldRecordValues;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(recordPosition, SeekOrigin.Begin);
                oldRecordValues = this.ReadRecordFirstFourFields(reader);
            }

            using BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true);
            this.fileStream.Seek(recordPosition, SeekOrigin.Begin);
            this.WriteArguments(arguments, binaryWriter);
        }

        private FileCabinetRecord ReadRecordFirstFourFields(BinaryReader reader)
        {
            var currentRecord = new FileCabinetRecord();
            reader.ReadInt16();
            currentRecord.Id = reader.ReadInt32();
            currentRecord.FirstName = reader.ReadString().ToUpperInvariant();
            this.MoveStreamToNextPositionAfterString(currentRecord.FirstName);
            currentRecord.LastName = reader.ReadString().ToUpperInvariant();
            this.MoveStreamToNextPositionAfterString(currentRecord.LastName);
            int year = reader.ReadInt32();
            int month = reader.ReadInt32();
            int day = reader.ReadInt32();
            currentRecord.DateOfBirth = new DateTime(year, month, day);
            return currentRecord;
        }

        private void SetRecordStatusDelete(long recordPosition)
        {
            short status;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(recordPosition, SeekOrigin.Begin);
                status = reader.ReadInt16();
            }

            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(recordPosition, SeekOrigin.Begin);
                status |= DeleteMask;
                binaryWriter.Write(status);
            }
        }
    }
}
