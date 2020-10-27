using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Representes predicates factory class.
    /// </summary>
    public static class PredicatesFactory
    {
        /// <summary>
        /// Creates predicate using specified filters.
        /// </summary>
        /// <param name="filtersDictionary">The dictionary that contains key as names of FileCabinetRecord properties and set of values.</param>
        /// <param name="isAndAlso">true if predicates combining method is AndAlso, false if predicates combining method is OrElse.</param>
        /// <returns>A condition.</returns>
        internal static Func<FileCabinetRecord, bool> GetPredicate(Dictionary<string, List<object>> filtersDictionary, bool isAndAlso)
        {
            if (filtersDictionary.Count is 0 || filtersDictionary is null)
            {
                return null;
            }

            var expressionsList = new List<Func<FileCabinetRecord, bool>>();
            foreach (var item in filtersDictionary)
            {
                List<object> valuesList = item.Value;
                foreach (var value in valuesList)
                {
                    Func<FileCabinetRecord, bool> expression = GetWhereExpression(item.Key, value);
                    expressionsList.Add(expression);
                }
            }

            Func<FileCabinetRecord, bool> predicate;
            if (expressionsList.Count == 1)
            {
                predicate = expressionsList[0];
            }
            else
            {
                predicate = CombinePredicates(expressionsList, isAndAlso);
            }

            return predicate;
        }

        private static Func<T, bool> CombinePredicates<T>(List<Func<T, bool>> predicates, bool isAndAlso)
        {
            int predicatesCount = predicates.Count;

            if (predicatesCount == 0)
            {
                return null;
            }

            Func<T, bool> predicateToReturn = predicates[0];

            if (predicatesCount == 1)
            {
                return predicateToReturn;
            }

            return (T item) =>
            {
                bool result = isAndAlso;
                if (isAndAlso)
                {
                    for (int i = 0; i < predicatesCount; i++)
                    {
                        result &= predicates[i](item);
                    }
                }
                else
                {
                    for (int i = 0; i < predicatesCount; i++)
                    {
                        result |= predicates[i](item);
                    }
                }

                return result;
            };
        }

        private static Func<FileCabinetRecord, bool> GetWhereExpression(string propertyName, object value)
        {
            Type recordType = typeof(FileCabinetRecord);
            MethodInfo propertyGet = recordType.GetProperty(propertyName).GetGetMethod();
            ParameterExpression record = Expression.Parameter(recordType, nameof(FileCabinetRecord));
            var valueType = value.GetType();
            Expression left;
            if (valueType.Equals(typeof(char)))
            {
                var leftPropertyGet = Expression.Property(record, propertyGet);
                var toStringMethod = valueType.GetMethod("ToString", Array.Empty<Type>());
                left = Expression.Call(leftPropertyGet, toStringMethod);
                valueType = typeof(string);
                value = value.ToString();
            }
            else
            {
                left = Expression.Property(record, propertyGet);
            }

            ConstantExpression right = Expression.Constant(value, valueType);
            Expression equalToConstantValue;
            if (valueType.Equals(typeof(string)))
            {
                equalToConstantValue = GetExpressionForStrings(left, valueType, right);
            }
            else
            {
                equalToConstantValue = Expression.Equal(left, right);
            }

            var whereExpression = Expression.Lambda<Func<FileCabinetRecord, bool>>(equalToConstantValue, record);
            return whereExpression.Compile();
        }

        private static Expression GetExpressionForStrings(Expression left, Type valueType, ConstantExpression right)
        {
            Expression equalToConstantValue;
            var stringEqualsMethod = valueType.GetMethod("Equals", new Type[] { typeof(string), typeof(StringComparison) });
            var stringComparison = Expression.Constant(StringComparison.InvariantCultureIgnoreCase, typeof(StringComparison));
            Expression[] arguments = new Expression[] { right, stringComparison };
            equalToConstantValue = Expression.Call(left, stringEqualsMethod, arguments);
            return equalToConstantValue;
        }
    }
}
