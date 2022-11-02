// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using Beyond.Extensions.ExpressionExtended;
using Beyond.Extensions.ObjectExtended;
using Beyond.Extensions.StringExtended;

namespace Beyond.Extensions.DataReaderExtended;

public static class DataReaderExtensions
{
    public static IEnumerable<IDataRecord> AsEnumerable(this IDataReader reader)
    {
        while (reader.Read()) yield return reader;
    }

    public static bool ContainsColumn(this IDataReader @this, int columnIndex)
    {
        try
        {
            // Check if FieldCount is implemented first
            return @this.FieldCount > columnIndex;
        }
        catch (Exception)
        {
            try
            {
                return @this[columnIndex] != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public static bool ContainsColumn(this IDataReader @this, string columnName)
    {
        try
        {
            // Check if GetOrdinal is implemented first
            return @this.GetOrdinal(columnName) != -1;
        }
        catch (Exception)
        {
            try
            {
                return @this[columnName] != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public static IDataReader ForEach(this IDataReader @this, Action<IDataReader> action)
    {
        while (@this.Read()) action(@this);

        return @this;
    }

    public static T? Get<T>(this IDataReader reader, string field)
    {
        return reader.Get(field, default(T));
    }

    public static T? Get<T>(this IDataReader reader, string field, T? defaultValue)
    {
        var value = reader[field];
        if (value == DBNull.Value)
            return defaultValue;

        if (value is T value1)
            return value1;

        return value.ConvertTo(defaultValue);
    }

    public static bool GetBoolean(this IDataReader reader, string field)
    {
        return reader.GetBoolean(field, false);
    }

    public static bool GetBoolean(this IDataReader reader, string field, bool defaultValue)
    {
        var value = reader[field];
        return value is bool b ? b : defaultValue;
    }

    public static byte[]? GetBytes(this IDataReader reader, string field)
    {
        return reader[field] as byte[];
    }

    public static IEnumerable<string> GetColumnNames(this IDataRecord @this)
    {
        return Enumerable.Range(0, @this.FieldCount)
            .Select(@this.GetName);
    }

    public static DateTime GetDateTime(this IDataReader reader, string field)
    {
        return reader.GetDateTime(field, DateTime.MinValue);
    }

    public static DateTime GetDateTime(this IDataReader reader, string field, DateTime defaultValue)
    {
        var value = reader[field];
        return value is DateTime time ? time : defaultValue;
    }

    public static DateTimeOffset GetDateTimeOffset(this IDataReader reader, string field)
    {
        return new DateTimeOffset(reader.GetDateTime(field), TimeSpan.Zero);
    }

    public static DateTimeOffset GetDateTimeOffset(this IDataReader reader, string field, DateTimeOffset defaultValue)
    {
        var dt = reader.GetDateTime(field);
        return dt != DateTime.MinValue ? new DateTimeOffset(dt, TimeSpan.Zero) : defaultValue;
    }

    public static decimal GetDecimal(this IDataReader reader, string field)
    {
        return reader.GetDecimal(field, 0);
    }

    public static decimal GetDecimal(this IDataReader reader, string field, long defaultValue)
    {
        var value = reader[field];
        return value is decimal value1 ? value1 : defaultValue;
    }

    public static Guid GetGuid(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is Guid guid ? guid : Guid.Empty;
    }

    public static int GetInt32(this IDataReader reader, string field)
    {
        return reader.GetInt32(field, 0);
    }

    public static int GetInt32(this IDataReader reader, string field, int defaultValue)
    {
        var value = reader[field];
        return value is int i ? i : defaultValue;
    }

    public static long GetInt64(this IDataReader reader, string field)
    {
        return reader.GetInt64(field, 0);
    }

    public static long GetInt64(this IDataReader reader, string field, int defaultValue)
    {
        var value = reader[field];
        return value is long l ? l : defaultValue;
    }

    public static bool? GetNullableBoolean(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is bool b ? b : null;
    }

    public static DateTime? GetNullableDateTime(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is DateTime time ? time : null;
    }

    public static DateTimeOffset? GetNullableDateTimeOffset(this IDataReader reader, string field)
    {
        var dt = reader.GetNullableDateTime(field);
        return dt != null ? new DateTimeOffset(dt.Value, TimeSpan.Zero) : null;
    }

    public static decimal? GetNullableDecimal(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is decimal value1 ? value1 : null;
    }

    public static Guid? GetNullableGuid(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is Guid guid ? guid : null;
    }

    public static int? GetNullableInt32(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is int i ? i : null;
    }

    public static long? GetNullableInt64(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value is long l ? l : null;
    }

    public static string? GetString(this IDataReader reader, string field)
    {
        return reader.GetString(field, null);
    }

    public static string? GetString(this IDataReader reader, string field, string? defaultValue)
    {
        var value = reader[field];
        return value as string ?? defaultValue;
    }

    public static Type? GetType(this IDataReader reader, string field)
    {
        return reader.GetType(field, null);
    }

    public static Type? GetType(this IDataReader reader, string field, Type? defaultValue)
    {
        var classType = reader.GetString(field);
        if (classType.IsNotEmpty())
        {
            if (classType == null) return defaultValue;
            var type = Type.GetType(classType);
            if (type != null)
                return type;
        }

        return defaultValue;
    }

    public static object? GetTypeInstance(this IDataReader reader, string field)
    {
        return reader.GetTypeInstance(field, null);
    }

    public static object? GetTypeInstance(this IDataReader reader, string field, Type? defaultValue)
    {
        var type = reader.GetType(field, defaultValue);
        return type != null ? Activator.CreateInstance(type) : null;
    }

    public static T? GetTypeInstance<T>(this IDataReader reader, string field) where T : class
    {
        return reader.GetTypeInstance(field, null) as T;
    }

    public static T? GetTypeInstanceSafe<T>(this IDataReader reader, string field, Type type) where T : class
    {
        var instance = reader.GetTypeInstance(field, null) as T;
        return instance ?? Activator.CreateInstance(type) as T;
    }

    public static T GetTypeInstanceSafe<T>(this IDataReader reader, string field) where T : class, new()
    {
        var instance = reader.GetTypeInstance(field, null) as T;
        return instance ?? new T();
    }

    public static T GetValueAs<T>(this IDataReader @this, int index)
    {
        return (T)@this.GetValue(index);
    }

    public static T GetValueAs<T>(this IDataReader @this, string columnName)
    {
        return (T)@this.GetValue(@this.GetOrdinal(columnName));
    }

    public static T? GetValueAsOrDefault<T>(this IDataReader @this, int index)
    {
        try
        {
            return (T)@this.GetValue(index);
        }
        catch
        {
            return default;
        }
    }

    public static T GetValueAsOrDefault<T>(this IDataReader @this, int index, T defaultValue)
    {
        try
        {
            return (T)@this.GetValue(index);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T GetValueAsOrDefault<T>(this IDataReader @this, int index,
        Func<IDataReader, int, T> defaultValueFactory)
    {
        try
        {
            return (T)@this.GetValue(index);
        }
        catch
        {
            return defaultValueFactory(@this, index);
        }
    }

    public static T? GetValueAsOrDefault<T>(this IDataReader @this, string columnName)
    {
        try
        {
            return (T)@this.GetValue(@this.GetOrdinal(columnName));
        }
        catch
        {
            return default;
        }
    }

    public static T GetValueAsOrDefault<T>(this IDataReader @this, string columnName, T defaultValue)
    {
        try
        {
            return (T)@this.GetValue(@this.GetOrdinal(columnName));
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T GetValueAsOrDefault<T>(this IDataReader @this, string columnName,
        Func<IDataReader, string, T> defaultValueFactory)
    {
        try
        {
            return (T)@this.GetValue(@this.GetOrdinal(columnName));
        }
        catch
        {
            return defaultValueFactory(@this, columnName);
        }
    }

    public static T? GetValueTo<T>(this IDataReader @this, int index)
    {
        return @this.GetValue(index).ConvertTo<T>();
    }

    public static T? GetValueTo<T>(this IDataReader @this, string columnName)
    {
        return @this.GetValue(@this.GetOrdinal(columnName)).ConvertTo<T>();
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, int index)
    {
        try
        {
            return @this.GetValue(index).ConvertTo<T>();
        }
        catch
        {
            return default;
        }
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, int index, T? defaultValue)
    {
        try
        {
            return @this.GetValue(index).ConvertTo<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, int index,
        Func<IDataReader, int, T> defaultValueFactory)
    {
        try
        {
            return @this.GetValue(index).ConvertTo<T>();
        }
        catch
        {
            return defaultValueFactory(@this, index);
        }
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, string columnName)
    {
        try
        {
            return @this.GetValue(@this.GetOrdinal(columnName)).ConvertTo<T>();
        }
        catch
        {
            return default;
        }
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, string columnName, T? defaultValue)
    {
        try
        {
            return @this.GetValue(@this.GetOrdinal(columnName)).ConvertTo<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T? GetValueToOrDefault<T>(this IDataReader @this, string columnName,
        Func<IDataReader, string, T> defaultValueFactory)
    {
        try
        {
            return @this.GetValue(@this.GetOrdinal(columnName)).ConvertTo<T>();
        }
        catch
        {
            return defaultValueFactory(@this, columnName);
        }
    }

    public static int IndexOf(this IDataRecord @this, string name,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        for (var i = 0; i < @this.FieldCount; i++)
            if (String.Compare(@this.GetName(i), name, stringComparison) == 0)
                return i;
        return -1;
    }

    public static bool IsDBNull(this IDataReader reader, string field)
    {
        var value = reader[field];
        return value == DBNull.Value;
    }

    public static int ReadAll(this IDataReader reader, Action<IDataReader> action)
    {
        var count = 0;
        while (reader.Read())
        {
            action(reader);
            count++;
        }

        return count;
    }

    public static TReturn? SafeColumnReader<TClass, TReturn>(this IDataReader reader,
        Expression<Func<TClass, object>> columnName)
        where TClass : class, new()
    {
        return reader.SafeColumnReader<TReturn>(columnName.GetPropertyName());
    }

    public static TReturn? SafeColumnReader<TReturn>(this IDataReader reader, string columnName)
    {
        try
        {
            object? retValue = null;
            var type = typeof(TReturn);
            var index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                if (type == typeof(bool))
                    retValue = reader.GetBoolean(index);

                if (type == typeof(byte))
                    retValue = reader.GetByte(index);

                if (type == typeof(byte[]))
                {
                    var byteArray = new byte[reader.GetBytes(index, 0, null, 0, int.MaxValue)];
                    reader.GetBytes(index, 0, byteArray, 0, byteArray.Length);
                    retValue = byteArray;
                }

                if (type == typeof(char))
                    retValue = reader.GetChar(index);

                if (type == typeof(char[]))
                {
                    var charArray = new char[reader.GetChars(index, 0, null, 0, int.MaxValue)];
                    reader.GetChars(index, 0, charArray, 0, charArray.Length);
                    retValue = charArray;
                }

                if (type == typeof(IDataReader))
                    retValue = reader.GetData(index);

                if (type == typeof(DateTime))
                    retValue = reader.GetDateTime(index);

                if (type == typeof(float))
                    retValue = reader.GetFloat(index);

                if (type == typeof(decimal))
                    retValue = reader.GetDecimal(index);

                if (type == typeof(double))
                    retValue = reader.GetDouble(index);

                if (type == typeof(Type))
                    retValue = reader.GetFieldType(index);

                if (type == typeof(Guid))
                    retValue = reader.GetGuid(index);

                if (type == typeof(short))
                    retValue = reader.GetInt16(index);

                if (type == typeof(int))
                    retValue = reader.GetInt32(index);

                if (type == typeof(long))
                    retValue = reader.GetInt64(index);

                if (type == typeof(string))
                    retValue = reader.GetString(index);

                if (type == typeof(object))
                    retValue = reader.GetValue(index);

                return (TReturn?)retValue;
            }

            return default;
        }
        catch (Exception ex)
        {
            if (ex is IndexOutOfRangeException) throw new IndexOutOfRangeException("The column name does not exist.");

            if (ex is InvalidCastException)
                throw new InvalidCastException(
                    "The return type, with the type of data in the database is not in compliance");
            return default;
        }
    }
}