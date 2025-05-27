using PortfolioValuationApp.Core.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PortfolioValuationApp.Core.Utilities
{
    public static class CsvReader
    {
        public static List<T> ReadCsv<T>(string filePath, char separator = ';') where T : new()
        {
            var records = new List<T>();

            if (!File.Exists(filePath))
            throw new FileNotFoundException(string.Format(LogMessages.CsvFileNotFound, filePath));


            try
            {
                using var reader = new StreamReader(filePath);
                var headers = reader.ReadLine()?.Split(separator);

                if (headers == null)
                    throw new InvalidDataException(LogMessages.CsvFileEmptyOrCorrupt);

                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(separator);
                    var obj = new T();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var prop = properties.FirstOrDefault(p =>
                            p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));

                        if (prop == null || i >= values.Length) continue;

                        try
                        {
                            object? safeValue = ConvertToType(values[i], prop.PropertyType);
                            prop.SetValue(obj, safeValue);
                        }
                        catch
                        {
                            //  logging 
                            continue;
                        }
                    }

                    records.Add(obj);
                }
            }
            catch (Exception ex)
            {
                //  logging 
                Console.WriteLine(string.Format(LogMessages.ErrorReadingCsv, ex.Message));
            }

            return records;
        }

        private static object? ConvertToType(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(int))
                return int.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(decimal))
                return decimal.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(DateTime))
                return DateTime.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(double))
                return double.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(bool))
                return bool.Parse(value);

            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);

            return Convert.ChangeType(value, targetType);
        }
    }
}
