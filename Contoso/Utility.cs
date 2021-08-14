using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace Contoso
{
    public static class Utility
    {
  /// <summary>
        /// Update entity.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="entity"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static T Update<T>(this T entity, IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            if (dictionary == null || entity == null) return entity;

            var tType = typeof(T);

            foreach (var (key, value) in dictionary)
            {
                var property = key.ToUpperFirst();
                var propertyType = tType.GetProperty(property);

                if (propertyType == null) continue;

                if (value == null)
                {
                    propertyType.SetValue(entity, null);
                }
                else
                {
                    var targetValue = propertyType.PropertyType == value.GetType()
                        ? value
                        : ChangeType(value, propertyType.PropertyType);

                    propertyType.SetValue(entity, targetValue);
                }
            }

            return entity;
        }

        /// <summary>
        /// Convert type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversion)
        {
            if (value is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.String:
                        return ChangeType(jsonElement.GetString(), typeof(string));
                    case JsonValueKind.Number:
                        return ChangeType(jsonElement.GetDouble(), typeof(double));
                    case JsonValueKind.True:
                        return ChangeType(jsonElement.GetBoolean(), typeof(bool));
                    case JsonValueKind.False:
                        return ChangeType(jsonElement.GetBoolean(), typeof(bool));
                    case JsonValueKind.Null:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
            if (conversion == null) throw new ArgumentNullException(nameof(conversion));

            if (!conversion.IsGenericType || conversion.GetGenericTypeDefinition() != typeof(Nullable<>))
                return Convert.ChangeType(value, conversion, CultureInfo.InvariantCulture);

            if (value == null)
                return null;

            conversion = Nullable.GetUnderlyingType(conversion);

            return Convert.ChangeType(value, conversion ?? throw new ArgumentNullException(nameof(conversion)), CultureInfo.InvariantCulture);
        }
    }
}