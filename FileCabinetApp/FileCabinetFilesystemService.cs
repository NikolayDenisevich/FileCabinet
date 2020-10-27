using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private const int DateOfBirthOffset = 246;
        private const int FirstNameOffset = 6;
        private const int LastNameOffset = 126;
        private const short DeleteMask = 4;
        private const string StatePath = @"C:\Cabinet\state.bin";
        private const string DefaultBinaryFilePath = @"C:\Cabinet\cabinet-records.db";
        private readonly FileStream fileStream;
        private readonly IRecordValidator<RecordArguments> validator;
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
            if (validator is null)
            {
                throw new ArgumentNullException($"{nameof(validator)} is null");
            }

            if (fileStream is null)
            {
                throw new ArgumentNullException($"{nameof(fileStream)} is null");
            }

            this.validator = validator;
            this.fileStream = fileStream;
            this.RestoreState();
        }

        /// <summary>
        /// Creates the FileCabinetServiceSnapshot instance.
        /// </summary>
        /// <param name="records">The records collection for export.</param>
        /// <returns>The FileCabinetServiceSnapshot instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public FileCabinetServiceSnapshot MakeSnapshot(IReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{nameof(records)} is null");
            }

            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Pugres the data file.
        /// </summary>
        /// <returns>Item1 is purged items count. Item2 total items before purge.</returns>
        public (int, int) Purge()
        {
            int totalRecordsCount = (int)(this.fileStream.Length / OneRecordFullLengthInBytes);
            (int, int) returnedValue;
            if (this.deletedRecordsCount > 0)
            {
                var notMarkedRecords = this.GetRecords();
                using (BinaryWriter writer = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
                {
                    writer.Seek(0, SeekOrigin.Begin);
                    foreach (var item in notMarkedRecords)
                    {
                        this.WriteRecord(item, writer);
                    }

                    this.fileStream.SetLength(OneRecordFullLengthInBytes * notMarkedRecords.Count);
                }

                returnedValue = (this.deletedRecordsCount, totalRecordsCount);
                this.deletedRecordsCount = 0;
            }
            else
            {
                returnedValue = (-1, -1);
            }

            return returnedValue;
        }

        /// <summary>
        /// Restrores all the containers after import from file.
        /// </summary>
        /// <param name="snapshot">FileCabinetServiceSnapshot instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        /// <returns>Restored records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} is null");
            }

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
            if (arguments is null)
            {
                throw new ArgumentNullException($"{nameof(arguments)} is null");
            }

            this.validator.ValidateArguments(arguments);
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                binaryWriter.Seek(sizeof(short), SeekOrigin.End);
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
            }

            this.isRecordReadedFromImportedFile = default;
            this.currentRecordsCount++;
            var lastWriteTime = DateTime.Now;
            this.SaveState(lastWriteTime);
            return this.maxId;
        }

        /// <summary>
        /// Edits a record in the file.
        /// </summary>
        /// <param name="arguments">File cabinet record arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when 'arguments' is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when arguments.Firstname, or arguments.Lastname, or arguments.City, or arguments.Street is null.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.Firstname, or arguments.Lsatname, or arguments.City, or arguments.Street trimmed length less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when arguments.DateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        /// <exception cref="ArgumentException">For parameter arguments.Gender permissible values are :'m', 'M', 'f', 'F'.</exception>
        /// <exception cref="ArgumentException">There is no record #{arguments.Id} in the list.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.ZipCode range is 1..9999.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Arguments.Salary range is 0..100 000.</exception>
        public void EditRecord(RecordArguments arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException($"{nameof(arguments)} is null");
            }

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

            (int, int, int) currentState;
            if (recordId == this.maxId)
            {
                currentState = this.GetActualStateFromFile();
                this.maxId = currentState.Item2;
            }

            this.currentRecordsCount--;
            this.deletedRecordsCount++;
            var lastWriteTime = DateTime.Now;
            this.SaveState(lastWriteTime);
        }

        /// <summary>
        /// Returns a sequence of records containing the date 'dateOfBirth'.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>A sequence of records containing the date 'dateOfBirth'.</returns>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                long recordsCount = this.fileStream.Length / OneRecordFullLengthInBytes;
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(i * OneRecordFullLengthInBytes, SeekOrigin.Begin);
                    short status = reader.ReadInt16();
                    if ((status &= DeleteMask) != DeleteMask)
                    {
                        this.fileStream.Seek(DateOfBirthOffset + (i * OneRecordFullLengthInBytes), SeekOrigin.Begin);
                        int currentYear = reader.ReadInt32();
                        int currentMonth = reader.ReadInt32();
                        int currentDay = reader.ReadInt32();

                        if (currentYear == dateOfBirth.Year && currentMonth == dateOfBirth.Month && currentDay == dateOfBirth.Day)
                        {
                            FileCabinetRecord record = this.ReadRecordFromFile(reader, i * OneRecordFullLengthInBytes);
                            list.Add(record);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>A sequence of records containing the name 'firstname'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when firstname is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstname length less than 2 or more than 60.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> list = this.CreateRecordsByNamesCollection(firstName, FirstNameOffset);

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns a sequence of records containing the name 'firstname'.
        /// </summary>
        /// <param name="lastName">Search key.</param>
        /// <returns>A sequence of records containing the name 'lastName'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when lastName length less than 2 or more than 60.</exception>
        public IReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> list = this.CreateRecordsByNamesCollection(lastName, LastNameOffset);

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns the collection of all records.
        /// </summary>
        /// <returns>The collection of all records.</returns>
        public IReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int recordsCounter = 0;
                int factor = 0;
                this.fileStream.Seek(factor, SeekOrigin.Begin);
                while (reader.PeekChar() != -1)
                {
                    short status = reader.ReadInt16();
                    if ((status &= DeleteMask) != DeleteMask)
                    {
                        FileCabinetRecord record = this.ReadRecordFromFile(reader, factor * OneRecordFullLengthInBytes);
                        list.Add(record);
                        recordsCounter++;
                    }

                    this.fileStream.Seek(++factor * OneRecordFullLengthInBytes, SeekOrigin.Begin);
                }

                reader.Close();
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
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
            long recordsCount = this.fileStream.Length / OneRecordFullLengthInBytes;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(i * OneRecordFullLengthInBytes, SeekOrigin.Begin);
                    short status = reader.ReadInt16();
                    if ((status &= DeleteMask) != DeleteMask)
                    {
                        int currentId = reader.ReadInt32();
                        if (currentId == id)
                        {
                            reader.Close();
                            return this.fileStream.Position - sizeof(int) - sizeof(short);
                        }
                    }
                }
            }

            return -1;
        }

        private List<FileCabinetRecord> CreateRecordsByNamesCollection(string name, int nameOffset)
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                long recordsCount = this.fileStream.Length / OneRecordFullLengthInBytes;
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(i * OneRecordFullLengthInBytes, SeekOrigin.Begin);
                    short status = reader.ReadInt16();
                    if ((status &= DeleteMask) != DeleteMask)
                    {
                        this.fileStream.Seek(nameOffset + (i * OneRecordFullLengthInBytes), SeekOrigin.Begin);
                        string currentFirstName = reader.ReadString();

                        if (currentFirstName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            FileCabinetRecord record = this.ReadRecordFromFile(reader, i * OneRecordFullLengthInBytes);
                            list.Add(record);
                        }
                    }
                }
            }

            return list;
        }

        private FileCabinetRecord ReadRecordById(int id)
        {
            long foundRecordPosition = this.FindRecordPosition(id);

            if (foundRecordPosition is -1)
            {
                return default;
            }

            FileCabinetRecord record;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                record = this.ReadRecordFromFile(reader, foundRecordPosition);
            }

            return record;
        }

        /// <summary>
        /// Returns current records count(Item1), max(Id)(Item2), removed records count (Item3).
        /// </summary>
        /// <returns>Item1 is current records count. Item2 is max(Id). Item3 is removed records count.</returns>
        private (int, int, int) GetActualStateFromFile()
        {
            int maxId = 0;
            int recordsCount = 0;
            int deletedCount = 0;
            int factor = 0;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(factor, SeekOrigin.Begin);
                if (reader.PeekChar() == -1)
                {
                    return (recordsCount, maxId, deletedCount);
                }

                while (reader.PeekChar() != -1)
                {
                    this.fileStream.Seek(OneRecordFullLengthInBytes * factor, SeekOrigin.Begin);
                    short status = reader.ReadInt16();
                    status &= DeleteMask;
                    if (status != DeleteMask)
                    {
                        int currentId = reader.ReadInt32();
                        if (currentId > maxId)
                        {
                            maxId = currentId;
                        }

                        recordsCount++;
                    }
                    else
                    {
                        deletedCount++;
                    }

                    this.fileStream.Seek(OneRecordFullLengthInBytes * ++factor, SeekOrigin.Begin);
                }
            }

            return (recordsCount, maxId, deletedCount);
        }

        private void EditExistingRecord(RecordArguments arguments, long recordPosition)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(sizeof(short) + recordPosition, SeekOrigin.Begin);
                this.WriteArguments(arguments, binaryWriter);
                binaryWriter.Close();
            }
        }

        private void SaveState(DateTime lastWrite)
        {
            FileStream stateStream;
            if (File.Exists(StatePath))
            {
                stateStream = File.Open(StatePath, FileMode.Open, FileAccess.Write);
            }
            else
            {
                stateStream = File.Open(StatePath, FileMode.CreateNew, FileAccess.Write);
            }

            using (BinaryWriter binaryWriter = new BinaryWriter(stateStream, Encoding.Unicode, false))
            {
                binaryWriter.Seek(0, SeekOrigin.Begin);
                binaryWriter.Write(this.currentRecordsCount);
                binaryWriter.Write(this.maxId);
                binaryWriter.Write(lastWrite.Year);
                binaryWriter.Write(lastWrite.Month);
                binaryWriter.Write(lastWrite.Day);
                binaryWriter.Write(lastWrite.Hour);
                binaryWriter.Write(lastWrite.Minute);
                binaryWriter.Write(lastWrite.Second);
                binaryWriter.Write(this.deletedRecordsCount);
            }
        }

        private void RestoreState()
        {
            FileStream stateStream;
            if (File.Exists(StatePath))
            {
                stateStream = File.Open(StatePath, FileMode.Open, FileAccess.Read);
                using (BinaryReader binaryReader = new BinaryReader(stateStream, Encoding.Unicode, false))
                {
                    stateStream.Seek(0, SeekOrigin.Begin);
                    int readedRecordsCount = binaryReader.ReadInt32();
                    int readedLastRecordsId = binaryReader.ReadInt32();
                    int readedYear = binaryReader.ReadInt32();
                    int readedMonth = binaryReader.ReadInt32();
                    int readedDay = binaryReader.ReadInt32();
                    int readedHour = binaryReader.ReadInt32();
                    int readedMinute = binaryReader.ReadInt32();
                    int readedSecond = binaryReader.ReadInt32();
                    int readedDeletedRecordsCount = binaryReader.ReadInt32();
                    DateTime readedWriteDate = new DateTime(readedYear, readedMonth, readedDay, readedHour, readedMinute, readedSecond);
                    DateTime actualWriteDate = File.GetLastWriteTime(DefaultBinaryFilePath);
                    TimeSpan span = readedWriteDate - actualWriteDate;
                    if (Math.Abs(span.TotalSeconds) < 3)
                    {
                        this.SetFieldsState(readedRecordsCount, readedLastRecordsId, readedDeletedRecordsCount);
                    }
                    else
                    {
                        var state = this.GetActualStateFromFile();
                        this.SetFieldsState(state.Item1, state.Item2, state.Item3);
                    }
                }
            }
            else
            {
                var state = this.GetActualStateFromFile();
                this.SetFieldsState(state.Item1, state.Item2, state.Item3);
            }
        }

        private void SetFieldsState(int readedRecordsCount, int readedLastRecordsId, int readedDeletedRecordsCount)
        {
            this.currentRecordsCount = readedRecordsCount;
            this.maxId = readedLastRecordsId;
            this.deletedRecordsCount = readedDeletedRecordsCount;
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

        private void WriteRecord(FileCabinetRecord record, BinaryWriter binaryWriter)
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
    }
}
