using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short zipCode, string city, string street, decimal salary, char gender)
        {
            this.ValidateArgumets(firstName, lastName, dateOfBirth, zipCode, city, street, salary, gender);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ZipCode = zipCode,
                City = city,
                Street = street,
                Salary = salary,
                Gender = gender,
            };

            this.list.Add(record);
            this.AddRecordToDictionary(firstName, record, this.firstNameDictionary);
            this.AddRecordToDictionary(lastName, record, this.lastNameDictionary);
            this.AddRecordToDictionary(dateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), record, this.dateOfBirthDictionary);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short zipCode, string city, string street, decimal salary, char gender)
        {
            this.ValidateArgumets(firstName, lastName, dateOfBirth, zipCode, city, street, salary, gender);
            FileCabinetRecord listRecord = this.list.Find(i => i.Id == id);
            if (listRecord is null)
            {
                throw new ArgumentException($"There is no record #{id} in the list.");
            }

            string oldFirstName = listRecord.FirstName;
            string oldLastName = listRecord.LastName;
            string oldDateOfBirth = listRecord.DateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo);
            listRecord.FirstName = firstName;
            listRecord.LastName = lastName;
            listRecord.DateOfBirth = dateOfBirth;
            listRecord.ZipCode = zipCode;
            listRecord.City = city;
            listRecord.Street = street;
            listRecord.Salary = salary;
            listRecord.Gender = gender;

            this.EditRecordInDictionary(oldFirstName, firstName, id, listRecord, this.firstNameDictionary);
            this.EditRecordInDictionary(oldLastName, lastName, id, listRecord, this.lastNameDictionary);
            this.EditRecordInDictionary(oldDateOfBirth, dateOfBirth.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), id, listRecord, this.dateOfBirthDictionary);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName) => this.GetRecordsFromDictionary(firstName, this.firstNameDictionary);

        public FileCabinetRecord[] FindByLastName(string lastName) => this.GetRecordsFromDictionary(lastName, this.lastNameDictionary);

        public FileCabinetRecord[] FindByDateOfBirth(DateTime date) => this.GetRecordsFromDictionary(date.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo), this.dateOfBirthDictionary);

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private void ValidateArgumets(string firstName, string lastName, DateTime dateOfBirth, short zipCode, string city, string street, decimal salary, char gender)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null");
            }

            string trimmedFirsName = firstName.Trim();

            if (trimmedFirsName.Length < 2 || trimmedFirsName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}.Length should be from 2 to 60. {nameof(firstName)} should not consist only of white-spaces characters");
            }

            if (lastName is null)
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null");
            }

            string trimmedLastName = lastName.Trim();

            if (trimmedLastName.Length < 2 || trimmedLastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}.Length should be from 2 to 60. {nameof(lastName)} should not consist only of white-spaces characters");
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth >= DateTime.Now)
            {
                throw new ArgumentException($"{nameof(dateOfBirth)} should be more than 01-Jan-1950 and not more than {DateTime.Now.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo)} or invalid date format input");
            }

            if (zipCode < 1 || zipCode > 9999)
            {
                throw new ArgumentOutOfRangeException($"{nameof(zipCode)} range is 1..9999");
            }

            if (city is null)
            {
                throw new ArgumentNullException($"{nameof(city)} is null");
            }

            string trimmedCity = city.Trim();

            if (trimmedCity.Length < 2 || trimmedCity.Length > 60)
            {
                throw new ArgumentException($"{nameof(city)}.Length should be from 2 to 60. {nameof(city)} should not consist only of white-spaces characters");
            }

            if (street is null)
            {
                throw new ArgumentNullException($"{nameof(street)} is null");
            }

            string trimmedStreet = street.Trim();

            if (trimmedStreet.Length < 2 || trimmedStreet.Length > 60)
            {
                throw new ArgumentException($"{nameof(street)}.Length should be from 2 to 60. {nameof(street)} should not consist only of white-spaces characters");
            }

            if (salary < 0 || salary > 100000)
            {
                throw new ArgumentOutOfRangeException($"{nameof(salary)} range is 0..100 000");
            }

            if (gender != 'm' && gender != 'M' && gender != 'f' && gender != 'F')
            {
                throw new ArgumentException($"{nameof(gender)} permissible values are :'m', 'M', 'f', 'F'.");
            }
        }

        private FileCabinetRecord[] GetRecordsFromDictionary(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            dictionary.TryGetValue(key.ToUpperInvariant(), out valuesList);
            return (valuesList is null) ? Array.Empty<FileCabinetRecord>() : valuesList.ToArray();
        }

        private void EditRecordInDictionary(string oldName, string newName, int id, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            if (oldName.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
            {
                dictionary.TryGetValue(oldName.ToUpperInvariant(), out valuesList);
                FileCabinetRecord dictionaryRecord = valuesList.Find(i => i.Id == id);
                dictionaryRecord = record;
            }
            else
            {
                dictionary.TryGetValue(oldName.ToUpperInvariant(), out valuesList);
                FileCabinetRecord oldDictionaryRecord = valuesList.Find(i => i.Id == id);
                valuesList.Remove(oldDictionaryRecord);
            }

            if (dictionary.TryGetValue(newName.ToUpperInvariant(), out valuesList))
            {
                valuesList.Add(record);
            }
            else
            {
                valuesList = new List<FileCabinetRecord>();
                valuesList.Add(record);
                dictionary.Add(newName.ToUpperInvariant(), valuesList);
            }
        }

        private void AddRecordToDictionary(string name, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            List<FileCabinetRecord> valuesList;
            if (dictionary.TryGetValue(name.ToUpperInvariant(), out valuesList))
            {
                valuesList.Add(record);
            }
            else
            {
                valuesList = new List<FileCabinetRecord>();
                valuesList.Add(record);
                dictionary.Add(name.ToUpperInvariant(), valuesList);
            }
        }
    }
}
