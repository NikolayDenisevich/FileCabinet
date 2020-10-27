using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides root node for FileCabinetRecords serialization.
    /// </summary>
    [XmlRoot("records")]
    public class XmlHelper
    {
        /// <summary>
        /// Records set for serialization.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable CA1051 // Do not declare visible instance fields
        [XmlElement("record")]
        public Record[] Records;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Fields should be private
    }

    /// <summary>
    /// Provides FileCabinetRecord for xml searization.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public class Record
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets or sets the ID value of this instance.
        /// </summary>
        /// <value>
        /// The ID value of this instance.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets full name value of this instance.
        /// </summary>
        /// <value>
        /// Full name value of this instance.
        /// </value>
        [XmlElement("name")]
        public FullName Name { get; set; }

        /// <summary>
        /// Gets or sets the Date of Birth value of this instance.
        /// </summary>
        /// <value>
        /// The Date of Birth value of this instance.
        /// </value>
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets full address value of this instance.
        /// </summary>
        /// <value>
        /// The full address value of this instance.
        /// </value>
        [XmlElement("address")]
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets the Salary value of this instance.
        /// </summary>
        /// <value>
        /// The Salary value of this instance.
        /// </value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets the Gender value of this instance.
        /// </summary>
        /// <value>
        /// The Gender value of this instance.
        /// </value>
        [XmlElement("gender")]
        public char Gender { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Id}, {this.Name.FirstName}, {this.Name.LastName}, {this.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, " +
                $"{this.Address.ZipCode}, {this.Address.City}, {this.Address.Street}, {this.Salary}, {this.Gender}";
        }
    }

    /// <summary>
    /// Provides element FullName for FileCabinetRecordXml.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public class FullName
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets or sets the Fist Name value of this instance.
        /// </summary>
        /// <value>
        /// The Fist Name value of this instance.
        /// </value>
        [XmlAttribute]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Last Name value of this instance.
        /// </summary>
        /// <value>
        /// The Last Name value of this instance.
        /// </value>
        [XmlAttribute]
        public string LastName { get; set; }
    }

    /// <summary>
    /// Provides element Address for FileCabinetRecordXml.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public class Address
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets or sets the Zip Code value of this instance.
        /// </summary>
        /// <value>
        /// The Zip Code value of this instance.
        /// </value>
        [XmlAttribute]
        public short ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the City value of this instance.
        /// </summary>
        /// <value>
        /// The City value of this instance.
        /// </value>
        [XmlAttribute]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Street value of this instance.
        /// </summary>
        /// <value>
        /// The Street value of this instance.
        /// </value>
        [XmlAttribute]
        public string Street { get; set; }
    }
}
