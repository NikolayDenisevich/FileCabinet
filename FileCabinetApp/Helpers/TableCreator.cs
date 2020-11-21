using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides table creator from collection of type T.
    /// </summary>
    public static class TableCreator
    {
        private const char PlusLiteral = '+';
        private const char MinusLiteral = '-';
        private const string DatePattern = "yyyy-MMM-dd";
        private static Dictionary<string, int> tableSettings;
        private static Dictionary<string, string> possibleSelectors;
        private static int settingsValuesSum;

        /// <summary>
        /// Creates table from collection of type T.
        /// </summary>
        /// <param name="collection">Elements collection of type T.</param>
        /// <param name="selectors">A set of posible columns names.</param>
        /// <param name="writer">Output stream for writing a table.</param>
        /// <typeparam name="T">Collection elements type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when collection is null. -or- selectors is null. -or- writer is null.</exception>
        public static void CreateTable<T>(this IEnumerable<T> collection, Dictionary<string, string> selectors, TextWriter writer)
        {
            if (collection is null)
            {
                return;
            }

            possibleSelectors = selectors ?? throw new ArgumentNullException($"{nameof(selectors)}");
            writer = writer ?? throw new ArgumentNullException($"{nameof(writer)}");
            Type type = typeof(T);
            var properties = type.GetProperties();
            InitializeTableSettings();
            SetSettings(collection, properties);
            string horizontalLine = BuildHorizontalString();
            writer.Write(horizontalLine);
            string head = BuildHeader(properties);

            writer.Write(head);
            writer.Write(horizontalLine);

            foreach (var item in collection)
            {
                writer.Write(BuildRow(item, properties));
                writer.Write(horizontalLine);
            }
        }

        private static void InitializeTableSettings()
        {
            tableSettings = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in possibleSelectors)
            {
                if (item.Value is null)
                {
                    continue;
                }

                tableSettings.Add(item.Key, 0);
            }
        }

        private static void SetSettings<T>(IEnumerable<T> collection, PropertyInfo[] properties)
        {
            const int ExtendedSymbolsCount = 2;
            foreach (var property in properties)
            {
                if (tableSettings.ContainsKey(property.Name))
                {
                    int propertyNameLangth = property.Name.Length;
                    int maxLength = collection.Max(m => property.GetValue(m).ToString().Length);
                    maxLength = maxLength < propertyNameLangth ? propertyNameLangth + ExtendedSymbolsCount : maxLength;
                    tableSettings[property.Name] = maxLength;
                    settingsValuesSum += maxLength;
                }
            }
        }

        private static void AppendByRight(StringBuilder builder, int fieldMaxLength, string valueString)
        {
            int spacesCount = fieldMaxLength - valueString.Length;
            builder.Append(' ', spacesCount);
            builder.Append(valueString);
        }

        private static void AppendCharAndStringByLeft(StringBuilder builder, int fieldMaxLength, object value)
        {
            string valueString = value.ToString();
            builder.Append(valueString);
            int spacesCount = fieldMaxLength - valueString.Length;
            builder.Append(' ', spacesCount);
        }

        private static string BuildRow<T>(T element, PropertyInfo[] typeProperties)
        {
            var builder = new StringBuilder(settingsValuesSum + (typeProperties.Length * 2));
            builder.Append('|');
            foreach (var kvp in tableSettings)
            {
                foreach (var property in typeProperties)
                {
                    if (kvp.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Type propertyType = property.PropertyType;
                        object value = property.GetValue(element);
                        if (propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(char)))
                        {
                            AppendCharAndStringByLeft(builder, kvp.Value, value);
                        }
                        else if (propertyType.Equals(typeof(DateTime)))
                        {
                            var dateValue = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                            string valueString = dateValue.ToString(DatePattern, CultureInfo.InvariantCulture);
                            AppendByRight(builder, kvp.Value, valueString);
                        }
                        else
                        {
                            AppendByRight(builder, kvp.Value, value.ToString());
                        }

                        builder.Append('|');

                        break;
                    }
                }
            }

            builder.AppendLine();
            return builder.ToString();
        }

        private static string BuildHorizontalString()
        {
            var builder = new StringBuilder(settingsValuesSum + (tableSettings.Count * 2));
            builder.Append(PlusLiteral);
            foreach (var kvp in tableSettings)
            {
                builder.Append(MinusLiteral, kvp.Value);
                builder.Append(PlusLiteral);
            }

            builder.AppendLine();
            return builder.ToString();
        }

        private static string BuildHeader(PropertyInfo[] typeProperties)
        {
            var builder = new StringBuilder(settingsValuesSum + (typeProperties.Length * 2));
            builder.Append('|');
            foreach (var kvp in tableSettings)
            {
                int spacesCount = kvp.Value - kvp.Key.Length;
                bool spacesCountIsEven = (spacesCount & 1) == 0;
                spacesCount /= 2;
                if (spacesCountIsEven)
                {
                    builder.Append(' ', spacesCount);
                    builder.Append(kvp.Key);
                    builder.Append(' ', spacesCount);
                }
                else
                {
                    builder.Append(' ', spacesCount);
                    builder.Append(kvp.Key);
                    builder.Append(' ', spacesCount + 1);
                }

                builder.Append('|');
            }

            builder.AppendLine();
            return builder.ToString();
        }
    }
}
