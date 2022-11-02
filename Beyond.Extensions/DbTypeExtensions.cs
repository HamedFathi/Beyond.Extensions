// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.DbTypeExtended;

public static class DbTypeExtensions
{
    public static Type ToType(this DbType dbType)
    {
        var typeMap = new Dictionary<DbType, Type>
        {
            [DbType.SByte] = typeof(sbyte),
            [DbType.Int16] = typeof(short),
            [DbType.UInt16] = typeof(ushort),
            [DbType.Int32] = typeof(int),
            [DbType.UInt32] = typeof(uint),
            [DbType.Int64] = typeof(long),
            [DbType.UInt64] = typeof(ulong),
            [DbType.Single] = typeof(float),
            [DbType.Double] = typeof(double),
            [DbType.Decimal] = typeof(decimal),
            [DbType.Boolean] = typeof(bool),
            [DbType.String] = typeof(string),
            [DbType.StringFixedLength] = typeof(char),
            [DbType.Guid] = typeof(Guid),
            [DbType.DateTime] = typeof(DateTime),
            [DbType.DateTimeOffset] = typeof(DateTimeOffset),
            [DbType.Binary] = typeof(byte[]),
            [DbType.Byte] = typeof(byte?),
            [DbType.SByte] = typeof(sbyte?),
            [DbType.Int16] = typeof(short?),
            [DbType.UInt16] = typeof(ushort?),
            [DbType.Int32] = typeof(int?),
            [DbType.UInt32] = typeof(uint?),
            [DbType.Int64] = typeof(long?),
            [DbType.UInt64] = typeof(ulong?),
            [DbType.Single] = typeof(float?),
            [DbType.Double] = typeof(double?),
            [DbType.Decimal] = typeof(decimal?),
            [DbType.Boolean] = typeof(bool?),
            [DbType.StringFixedLength] = typeof(char?),
            [DbType.Guid] = typeof(Guid?),
            [DbType.DateTime] = typeof(DateTime?),
            [DbType.DateTimeOffset] = typeof(DateTimeOffset?)
            // ,[System.Data.DbType.Binary] = typeof(System.Data.Linq.Binary)
        };
        return typeMap[dbType];
    }
}