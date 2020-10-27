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
    internal class ServiceCommandHandlerBase : CommandHandlerBase
    {
        internal static void InitializeDictionaryByPropertiesNames(Dictionary<string, string> dictionary)
        {
            var properties = typeof(FileCabinetRecord).GetProperties();
            foreach (var property in properties)
            {
                dictionary.Add(property.Name, null);
            }
        }
    }
}
