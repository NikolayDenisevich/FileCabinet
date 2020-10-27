using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class Program
    {
        private const string HintMessage = "Input error. Example to generate: " +
            "\n1. FileCabinetGenerator.exe --output-type=csv --output=d:\\data\\records.csv --records-amount=10000 --start-id=30." +
            "\n2. FileCabinetGenerator.exe -t xml -o c:\\users\\myuser\\records.xml -a 5000 -i 45.";
        private const string InvalidParameterMessage = "Invalid parameter for ";
        private const int MaxRecordAmont = 1_000_000;
        private const int ExitCodeIncorrectParameters = -100500;

        private static readonly Tuple<string, Action<string, string>>[] commands = new Tuple<string, Action<string, string>>[]
        {
            new Tuple<string, Action<string, string>>("--output-type", SetOutputType),
            new Tuple<string, Action<string, string>>("-t", SetOutputType),
            new Tuple<string, Action<string, string>>("--output", SetOutputFileName),
            new Tuple<string, Action<string, string>>("-o", SetOutputFileName),
            new Tuple<string, Action<string, string>>("--records-amount", SetRecordsAmount),
            new Tuple<string, Action<string, string>>("-a", SetRecordsAmount),
            new Tuple<string, Action<string, string>>("--start-id", SetStartId),
            new Tuple<string, Action<string, string>>("-i", SetStartId),
        };

        private static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        private static FileCabinetRecord[] records;
        private static Record[] recordsXml;
        private static DateTime today = DateTime.Now;
        private static Random random;

        private static bool? IsCsv { get; set; }

        private static string FilePath { get; set; }

        private static int RecordsAmount { get; set; }

        private static int StartId { get; set; }


        static void Main(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                Console.WriteLine(Program.HintMessage);
                Console.ReadKey();
                Environment.Exit(Program.ExitCodeIncorrectParameters);
            }
            
            Console.WriteLine("Input data:");
            foreach (var item in args)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(new string('-', 50));

            GetCommandsPairs(args);
            for (int i = 0; i < commands.Length; i++)
            {
                string command = commands[i].Item1;
                if (keyValuePairs.ContainsKey(command))
                {
                    string parameters;
                    bool isGotValue = keyValuePairs.TryGetValue(command, out parameters);
                    if (isGotValue)
                    {
                        commands[i].Item2(parameters, command);
                    }
                }
            }

            if (AreAllPropertiesFilled())
            {
                GenerateRecords();
                if ((bool)IsCsv)
                {
                    WriteToCsvFile();
                    Console.WriteLine($"{RecordsAmount} records were written to {Path.GetFileName(FilePath)}.");
                }
                else
                {
                    WriteToXmlFile();
                    Console.WriteLine($"{RecordsAmount} records were written to {Path.GetFileName(FilePath)}.");
                }
            }
            else
            {
                Console.WriteLine($"Some parameter(s) is wrong, please check your input.");
                Console.WriteLine(Program.HintMessage);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void WriteToXmlFile()
        {
            XmlHelper xmlData = new XmlHelper();
            xmlData.Records = recordsXml;
            XmlSerializer serializer = new XmlSerializer(typeof(XmlHelper));
            using var stream = File.Open(FilePath, FileMode.CreateNew, FileAccess.Write);
            using var streamWriter = new StreamWriter(stream, Encoding.Unicode);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(streamWriter, xmlData, ns);
        }

        public static Record[] GetRecordsXml()
        {
            return recordsXml;
        }

        private static void WriteToCsvFile()
        {
            using var stream = File.Open(FilePath, FileMode.CreateNew, FileAccess.Write);
            using var streamWriter = new StreamWriter(stream, Encoding.Unicode);
            foreach (var item in records)
            {
                streamWriter.WriteLine(item.ToString());
            }
        }

        private static void GenerateRecords()
        {
            random = new Random(Environment.TickCount);
            if ((bool)IsCsv)
            {
                GenerateFileCabinetRecordsCsv();
            }
            else
            {
                GenerateFileCabinetRecordsXml();
            }
        }

        private static void GenerateFileCabinetRecordsCsv()
        {
            int count = RecordsAmount;
            records = new FileCabinetRecord[count];
            for (int i = 0; i < count; i++)
            {
                var record = new FileCabinetRecord
                {
                    Id = StartId++,
                    FirstName = GetString(),
                    LastName = GetString(),
                    DateOfBirth = GetDate(),
                    ZipCode = (short)random.Next(1, 9999),
                    City = GetString(),
                    Street = GetString(),
                    Salary = (random.Next(100, 10_000_000)) / 100m,
                    Gender = GetChar()
                };
                records[i] = record;
            }
        }

        private static void GenerateFileCabinetRecordsXml()
        {
            int count = RecordsAmount;
            recordsXml = new Record[count];
            for (int i = 0; i < count; i++)
            {
                var recordXml = new Record
                {
                    Id = StartId++
                };
                FileCabinetApp.FullName fullname = new FileCabinetApp.FullName
                {
                    FirstName = GetString(),
                    LastName = GetString(),
                };
                recordXml.Name = fullname;
                recordXml.DateOfBirth = GetDate();
                FileCabinetApp.Address address = new FileCabinetApp.Address
                {
                    ZipCode = (short)random.Next(1, 9999),
                    City = GetString(),
                    Street = GetString(),
                };
                recordXml.Address = address;
                recordXml.Salary = (random.Next(100, 10_000_000)) / 100m;
                recordXml.Gender = GetChar();
                recordsXml[i] = recordXml;
            }
        }

        private static char GetChar()
        {
            int value = random.Next(0, 2);
            return value == 0 ? 'f' : 'm';
        }

        private static string GetString()
        {
            int length = random.Next(2, 60);
            char[] charArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                charArray[i] = (char)random.Next(97,123);
            }
            return new string(charArray);
        }

        private static DateTime GetDate()
        {
            int year = random.Next(1950, today.Year);
            int month = random.Next(1, 13);
            var day = month switch
            {
                1 => random.Next(1, 32),
                2 => random.Next(1, 29),
                3 => random.Next(1, 32),
                4 => random.Next(1, 31),
                5 => random.Next(1, 32),
                6 => random.Next(1, 31),
                7 => random.Next(1, 32),
                8 => random.Next(1, 32),
                9 => random.Next(1, 31),
                10 => random.Next(1, 32),
                11 => random.Next(1, 31),
                12 => random.Next(1, 32),
                _ => random.Next(1, 29),
            };

            return new DateTime(year, month, day);
        }

        private static void SetRecordsAmount(string parameters, string commandInfo)
        {
            int recordsAmount;
            bool isParsed = int.TryParse(parameters, out recordsAmount);
            if (!isParsed || recordsAmount > MaxRecordAmont || recordsAmount < 1)
            {
                Console.WriteLine("Records amount is more than 1_000_000");
                PrintInvalidMessageAndExit(commandInfo);
            }

            RecordsAmount = recordsAmount;
        }

        private static void SetStartId(string parameters, string commandInfo)
        {
            int id;
            bool isParsed = int.TryParse(parameters, out id);
            long idPlusRecordsAmount = (long)id + (long)RecordsAmount;
            if (!isParsed || idPlusRecordsAmount > int.MaxValue || id < 1)
            {
                Console.WriteLine($"Value {parameters} is incorrect or (StartID + recordsAmount) is more than int.MaxValue");
                PrintInvalidMessageAndExit(commandInfo);
            }

            StartId = id;
        }

        private static void PrintInvalidMessageAndExit(string commandInfo)
        {
            Console.WriteLine($"{Program.InvalidParameterMessage}{commandInfo}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine("Press any key and try again.");
            Console.ReadKey();
            Environment.Exit(ExitCodeIncorrectParameters);
        }

        private static void SetOutputFileName(string parameters, string commandInfo)
        {
            if (string.IsNullOrEmpty(parameters) || !Path.IsPathRooted(parameters) ||
                !Path.HasExtension(parameters) || !Path.IsPathFullyQualified(parameters))
            {
                PrintInvalidMessageAndExit(commandInfo);
            }

            FilePath = parameters;
        }

        private static void SetOutputType(string parameters, string commandInfo)
        {
            if (string.IsNullOrEmpty(parameters) ||
                !parameters.Equals("csv", StringComparison.InvariantCultureIgnoreCase) &&
                !parameters.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                PrintInvalidMessageAndExit(commandInfo);
            }

            if (parameters.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                IsCsv = true;
            }

            if (parameters.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                IsCsv = false;
            }
        }

        private static void GetCommandsPairs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains('='))
                {
                    var splitResult = args[i].Split("=");
                    if (splitResult.Length > 1)
                    {
                        keyValuePairs.Add(splitResult[0], splitResult[1]);
                    }
                }
                else
                {
                    if (i % 2 == 1)
                    {
                        keyValuePairs.Add(args[i - 1], args[i]);
                    }
                }
            }
        }

        private static bool AreAllPropertiesFilled()
        {
            return !(IsCsv is null) && !(FilePath is null) && !(RecordsAmount is 0) && !(StartId is 0);
        }
    }

}
