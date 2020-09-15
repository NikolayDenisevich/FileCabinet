using System;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short ZipCode { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public decimal Salary { get; set; }

        public char Gender { get; set; }
    }
}
