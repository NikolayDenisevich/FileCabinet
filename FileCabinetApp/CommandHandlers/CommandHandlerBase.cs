using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides CommandHandler base class.
    /// </summary>
    internal class CommandHandlerBase : ICommandHandler
    {
        private static string[] commands = new string[] { "help", "exit", "stat", "create", "list", "find", "export", "import", "purge", "insert", "delete", "update" };

        private ICommandHandler nextHandler;

        /// <summary>
        /// Sets next command handler in the chain.
        /// </summary>
        /// <param name="commandHandler">Next command handler in the chain.</param>
        /// <returns>CommandHandlerBase instance command handler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return this.nextHandler;
        }

        /// <summary>
        /// Handles command request.
        /// </summary>
        /// <param name="commandRequest">Request for handling.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
            else
            {
                var similarCommands = this.FindSimilarCommands(commandRequest.Command);
                if (!similarCommands.Any())
                {
                    PrintMissedCommandInfo(commandRequest.Command);
                }
                else
                {
                    PrintMostSimilarCommands(similarCommands);
                }
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintMostSimilarCommands(IEnumerable<string> commands)
        {
            var enumerator = commands.GetEnumerator();
            enumerator.MoveNext();
            var command = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                PrintMostSimilarCommands(command);
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

        private static void PrintMostSimilarCommands(string command)
        {
            Console.WriteLine($"The most similar command is:");
            Console.WriteLine($"\t{command}");
        }

        private static int[] CalculateSymbolsMathesInCommands(string input)
        {
            int[] counters = new int[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                if (input.Length > commands[i].Length)
                {
                    continue;
                }

                if (input.Length == 1)
                {
                    if (commands[i].StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                    {
                        counters[i]++;
                    }

                    continue;
                }

                foreach (char symbol in input)
                {
                    if (commands[i].Contains(symbol, StringComparison.InvariantCultureIgnoreCase))
                    {
                        counters[i]++;
                    }
                }
            }

            return counters;
        }

        private IEnumerable<string> FindSimilarCommands(string input)
        {
            int[] counters = CalculateSymbolsMathesInCommands(input);

            int max = counters.Max();
            if (max == 0)
            {
                yield break;
            }

            for (int i = 0; i < counters.Length; i++)
            {
                if (counters[i] == max)
                {
                    yield return commands[i];
                }
            }
        }
    }
}
