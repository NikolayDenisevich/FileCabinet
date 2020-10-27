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
        private const int FullDateOfBirthLengthInBytes = 12;
        private const int FirstNameOffset = 6;
        private const int LastNameOffset = 126;
        private readonly FileStream fileStream;
        private readonly IRecordValidator<RecordArguments> validator;

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

            int lastId = this.GetStat();
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                binaryWriter.Seek(sizeof(short), SeekOrigin.End);
                binaryWriter.Write(++lastId);
                this.WriteAllArgumentsExceptId(arguments, binaryWriter);
                binaryWriter.Close();
            }

            return lastId;
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

            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(sizeof(short) + foundRecordPosition, SeekOrigin.Begin);
                this.WriteAllArgumentsExceptId(arguments, binaryWriter);
                binaryWriter.Close();
            }
        }

        /// <summary>
        /// Returns a sequence of records containing the date 'dateOfBirth'.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>A sequence of records containing the date 'dateOfBirth'.</returns>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth is less than 01-Jan-1950 and more than now.</exception>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int recordsCount = this.GetStat();
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(DateOfBirthOffset + (i * OneRecordFullLengthInBytes), SeekOrigin.Begin);
                    int currentYear = reader.ReadInt32();
                    int currentMonth = reader.ReadInt32();
                    int currentDay = reader.ReadInt32();

                    if (currentYear == dateOfBirth.Year && currentMonth == dateOfBirth.Month && currentDay == dateOfBirth.Day)
                    {
                        long startReadPosition = this.fileStream.Position - FullDateOfBirthLengthInBytes - DateOfBirthOffset;
                        this.ReadRecordFromFileToBuffer(list, reader, startReadPosition);
                    }
                }

                reader.Close();
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
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
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> list = this.CreateRecordsByNamesCollection(lastName, LastNameOffset);

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns the collection of all records.
        /// </summary>
        /// <returns>The collection of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(sizeof(short), SeekOrigin.Begin);
                int recordsCounter = 0;
                while (reader.PeekChar() != -1)
                {
                    this.ReadRecordFromFileToBuffer(list, reader, recordsCounter * OneRecordFullLengthInBytes);
                    recordsCounter++;
                }

                reader.Close();
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns records count.
        /// </summary>
        /// <returns>Records count.</returns>
        public int GetStat()
        {
            int lastRecordId;
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(sizeof(short), SeekOrigin.Begin);
                if (reader.PeekChar() == -1)
                {
                    return 0;
                }

                int lastRecordIdOffset = OneRecordFullLengthInBytes - sizeof(short);
                this.fileStream.Seek(-lastRecordIdOffset, SeekOrigin.End);
                lastRecordId = reader.ReadInt32();
                reader.Close();
            }

            return lastRecordId;
        }

        private void ReadRecordFromFileToBuffer(List<FileCabinetRecord> buffer, BinaryReader reader, long fileStreamPosition)
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
            buffer.Add(record);
        }

        private void WriteAllArgumentsExceptId(RecordArguments arguments, BinaryWriter binaryWriter)
        {
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
            int recordsCount = this.GetStat();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(sizeof(short) + (i * OneRecordFullLengthInBytes), SeekOrigin.Begin);
                    int currentId = reader.ReadInt32();
                    if (currentId == id)
                    {
                        reader.Close();
                        return this.fileStream.Position - sizeof(short);
                    }
                }
            }

            return -1;
        }

        private List<FileCabinetRecord> CreateRecordsByNamesCollection(string firstName, int nameOffset)
        {
            var list = new List<FileCabinetRecord>();
            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int recordsCount = this.GetStat();
                for (int i = 0; i < recordsCount; i++)
                {
                    this.fileStream.Seek(nameOffset + (i * OneRecordFullLengthInBytes), SeekOrigin.Begin);
                    string currentFirstName = reader.ReadString();

                    if (currentFirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        long startReadPosition = this.fileStream.Position - ((currentFirstName.Length * BytesInOneUnicodeSymbol) + 1) - nameOffset;
                        this.ReadRecordFromFileToBuffer(list, reader, startReadPosition);
                    }
                }

                reader.Close();
            }

            return list;
        }
    }
}
