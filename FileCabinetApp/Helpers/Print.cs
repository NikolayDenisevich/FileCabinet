using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public static class Print
    {
        private const string HintMessage = "You should use this command with valid parameters and values. Enter 'help <commandname>' to get help.";

        /// <summary>
        /// Prints hint about parametrized commnds using.
        /// </summary>
        internal static void ParametrizedCommandHint()
        {
            Console.WriteLine(HintMessage);
        }

        internal static void MissedPropertyInfo(string propertyName)
        {
            Console.WriteLine($"There is no '{propertyName}' property.");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints incorrect syntax message.
        /// </summary>
        /// <param name="input">Incorrect data string.</param>
        internal static void IncorrectSyntax(string input)
        {
            Console.WriteLine($"Incorrect syntax: '{input}'");
            Print.ParametrizedCommandHint();
        }

        /// <summary>
        /// Prints incorrect syntax message.
        /// </summary>
        /// <param name="input">Incorrect data.</param>
        /// <param name="parameter">Incorrect parameter.</param>
        internal static void IncorrectSyntax(string input, string parameter)
        {
            Console.WriteLine($"Incorrect syntax: '{input}' : '{parameter}' or near '{parameter}'");
            Print.ParametrizedCommandHint();
        }

        /// <summary>
        /// Prints mismatch of parameters and (or) values message.
        /// </summary>
        /// <param name="parameters">Incorrect parameters.</param>
        /// <param name="values">Incorrect values.</param>
        internal static void MismatchOfParametersAndValues(string parameters, string values)
        {
            Console.WriteLine($"Mismatch of parameters and (or) values: '({parameters})' values ({values}) or comma using for float numbers");
        }

        internal static void OperationResult(IEnumerable<FileCabinetRecord> operatedRecords, string operationName)
        {
            const int ExtraSymbolsToRemoveBuidlerOffsetFromEnd = 2;
            const int ExtraSymbolsBuidlerCount = 2;
            var enumerator = operatedRecords.GetEnumerator();
            enumerator.MoveNext();
            var record = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                Print.OperationResult(record.Id, operationName);
                return;
            }

            var builder = new StringBuilder();
            builder.Append("Records ");
            foreach (var item in operatedRecords)
            {
                builder.Append($"#{item.Id}, ");
            }

            builder.Remove(builder.Length - ExtraSymbolsToRemoveBuidlerOffsetFromEnd, ExtraSymbolsBuidlerCount);
            builder.Append($" are {operationName}d.");
            Console.WriteLine(builder);
        }

        internal static void ValidationRulesUsageHint()
        {
            Console.WriteLine("<-v> parameter value is wrong or empty. Use <-v custom> or <--validation-rules=custom> " +
                "if you want to use custom validation rules (custom validation rules are for memory storage only.).");
        }

        internal static void StorageUsageHint()
        {
            Console.WriteLine("<-s> parameter value is wrong or empty. Use <-s file> or <--storage=file> if you want to use filesystem storage.");
        }

        internal static void StopWathActivated()
        {
            Console.WriteLine("StopWatch activated.");
        }

        internal static void LoggingActivated()
        {
            Console.WriteLine("Logging activated.");
        }

        internal static void UsingServerValidationRules(string serverValidationType)
        {
            Console.WriteLine($"Using {serverValidationType} validation rules.");
        }

        internal static void UsingStorageType(string storageType)
        {
            Console.WriteLine($"Using {storageType} storage.");
        }

        internal static void MissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        internal static void UseCommandWithoutParameters(string commandName)
        {
            Console.WriteLine($"Please, use '{commandName}' command without any parameters.");
        }

        internal static void RecordDoesntExists(string propertyName, string value)
        {
            Console.WriteLine($"There is no records with {propertyName} '{value}'");
        }

        internal static void RepeatedProperty(string propertyName)
        {
            Console.WriteLine($"property '{propertyName}' declared several times.");
        }

        internal static void NoRecordsWithFilters(string filters)
        {
            Console.WriteLine($"No records found using the specified filter(s) '{filters}'");
        }

        internal static void CannotSetValueToProperty(string propertyName)
        {
            Console.WriteLine($"Sorry, but you cannot set value to {propertyName}");
        }

        internal static void MostSimilarCommands(IEnumerable<string> commands)
        {
            var enumerator = commands.GetEnumerator();
            enumerator.MoveNext();
            var command = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                Print.MostSimilarCommands(command);
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("The most similar commands are:");
            foreach (var item in commands)
            {
                builder.AppendLine($"\t{item}");
            }

            Console.WriteLine(builder);
        }

        private static void MostSimilarCommands(string command)
        {
            Console.WriteLine($"The most similar command is:");
            Console.WriteLine($"\t{command}");
        }

        private static void OperationResult(int id, string operationName)
        {
            Console.WriteLine($"Record #{id} is {operationName}d.");
        }
    }
}
