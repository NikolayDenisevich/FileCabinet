using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides CommandHandler base class.
    /// </summary>
    internal class CommandHandlerBase : ICommandHandler
    {
        private static readonly string[] Commands = new string[]
        {
            "help", "exit", "stat", "create", "export", "import", "purge", "insert", "delete", "update", "select",
        };

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
        /// <exception cref="ArgumentNullException">Thrown when commandRequest is null.</exception>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            commandRequest = commandRequest ?? throw new ArgumentNullException(nameof(commandRequest));
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
            else
            {
                var similarCommands = this.FindSimilarCommands(commandRequest.Command);
                if (!similarCommands.Any())
                {
                    Print.MissedCommandInfo(commandRequest.Command);
                }
                else
                {
                    Print.MostSimilarCommands(similarCommands);
                }
            }
        }

        private static int[] CalculateSymbolsMathesInCommands(string input)
        {
            int[] counters = new int[Commands.Length];
            for (int i = 0; i < Commands.Length; i++)
            {
                if (input.Length > Commands[i].Length)
                {
                    continue;
                }

                if (input.Length == 1)
                {
                    if (Commands[i].StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                    {
                        counters[i]++;
                    }

                    continue;
                }

                foreach (char symbol in input)
                {
                    if (Commands[i].Contains(symbol, StringComparison.InvariantCultureIgnoreCase))
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
                    yield return Commands[i];
                }
            }
        }
    }
}
