using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes Print class.
    /// </summary>
    public static class Print
    {
        /// <summary>
        /// Prints hint about parametrized commnds using.
        /// </summary>
        internal static void ParametrizedCommandHint()
        {
            const string HintMessage = "You should use this command with valid parameters and values. Enter 'help <commandname>' to get help.";
            Console.WriteLine(HintMessage);
        }

        /// <summary>
        /// Prints message about missed property.
        /// </summary>
        /// <param name="propertyName">The name of missed property.</param>
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

        /// <summary>
        /// Prints message about operation result.
        /// </summary>
        /// <param name="operatedRecords">The set of records with which the operation was performed.</param>
        /// <param name="operationName">The name of operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when operatedRecords is null.</exception>
        internal static void OperationResult(IEnumerable<FileCabinetRecord> operatedRecords, string operationName)
        {
            operatedRecords = operatedRecords ?? throw new ArgumentNullException(nameof(operatedRecords));
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

        /// <summary>
        /// Prints  validation rules usage hint.
        /// </summary>
        internal static void ValidationRulesUsageHint()
        {
            Console.WriteLine("<-v> parameter value is wrong or empty. Use <-v custom> or <--validation-rules=custom> " +
                "if you want to use custom validation rules (custom validation rules are for memory storage only.).");
        }

        /// <summary>
        /// Prints storage usage hint.
        /// </summary>
        internal static void StorageUsageHint()
        {
            Console.WriteLine("<-s> parameter value is wrong or empty. Use <-s file> or <--storage=file> if you want to use filesystem storage.");
        }

        /// <summary>
        /// Prints message indicates that stopwatch is activatd.
        /// </summary>
        internal static void StopWathActivated()
        {
            Console.WriteLine("StopWatch activated.");
        }

        /// <summary>
        /// Prints message indicates that logging is activatd.
        /// </summary>
        internal static void LoggingActivated()
        {
            Console.WriteLine("Logging activated.");
        }

        /// <summary>
        /// Prints message whith type of service validation rules.
        /// </summary>
        /// <param name="serviceValidationType">The validation rules type.</param>
        internal static void UsingServiceValidationRules(string serviceValidationType)
        {
            Console.WriteLine($"Using {serviceValidationType} validation rules.");
        }

        /// <summary>
        /// Prints message whith type of storage.
        /// </summary>
        /// <param name="storageType">The data storage type.</param>
        internal static void UsingStorageType(string storageType)
        {
            Console.WriteLine($"Using {storageType} storage.");
        }

        /// <summary>
        /// Prints message about missed command.
        /// </summary>
        /// <param name="command">Missed command.</param>
        internal static void MissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints hint about parametless command using.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        internal static void UseCommandWithoutParameters(string commandName)
        {
            Console.WriteLine($"Please, use '{commandName}' command without any parameters.");
        }

        /// <summary>
        /// Prints message about repeated property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        internal static void RepeatedProperty(string propertyName)
        {
            Console.WriteLine($"The property '{propertyName}' declared several times.");
        }

        /// <summary>
        /// Prints message about the absence of records with the specified filters.
        /// </summary>
        /// <param name="filters">The specified filters string representation.</param>
        internal static void NoRecordsWithFilters(string filters)
        {
            Console.WriteLine($"No records found using the specified filter(s) '{filters}'");
        }

        /// <summary>
        /// Prints message about the absence of records.
        /// </summary>
        internal static void NoRecords()
        {
            Console.WriteLine($"There is no records in the list.");
        }

        /// <summary>
        /// Prints a message about the impossibility to assign a value to the specified property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        internal static void CannotSetValueToProperty(string propertyName)
        {
            Console.WriteLine($"Sorry, but you cannot set value to {propertyName}");
        }

        /// <summary>
        /// Prints a message about the most similar commands.
        /// </summary>
        /// <param name="commands">The most similar commands collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when commands is null.</exception>
        internal static void MostSimilarCommands(IEnumerable<string> commands)
        {
            commands = commands ?? throw new ArgumentNullException(nameof(commands));
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
