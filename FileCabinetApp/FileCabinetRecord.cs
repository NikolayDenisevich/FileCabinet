﻿using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides an object representation of FileCabinetRecord.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the ID value of this instance.
        /// </summary>
        /// <value>
        /// The ID value of this instance.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Fist Name value of this instance.
        /// </summary>
        /// <value>
        /// The Fist Name value of this instance.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Last Name value of this instance.
        /// </summary>
        /// <value>
        /// The Last Name value of this instance.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Date of Birth value of this instance.
        /// </summary>
        /// <value>
        /// The Date of Birth value of this instance.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the Zip Code value of this instance.
        /// </summary>
        /// <value>
        /// The Zip Code value of this instance.
        /// </value>
        public short ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the City value of this instance.
        /// </summary>
        /// <value>
        /// The City value of this instance.
        /// </value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Street value of this instance.
        /// </summary>
        /// <value>
        /// The Street value of this instance.
        /// </value>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the Salary value of this instance.
        /// </summary>
        /// <value>
        /// The Salary value of this instance.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets the Gender value of this instance.
        /// </summary>
        /// <value>
        /// The Gender value of this instance.
        /// </value>
        public char Gender { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return new StringBuilder()
                .Append($"{this.Id},")
                .Append($"{this.FirstName},")
                .Append($"{this.LastName},")
                .Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, ")
                .Append($"{this.ZipCode},")
                .Append($"{this.City},")
                .Append($"{this.Street},")
                .Append($"{this.Salary.ToString(CultureInfo.InvariantCulture)},")
                .Append($"{this.Gender}")
                .ToString();
        }
    }
}
