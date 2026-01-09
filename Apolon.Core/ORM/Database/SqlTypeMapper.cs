namespace Apolon.Core.ORM.Database
{
    public static class SqlTypeMapper
    {
        public static string GetPostgresType(Type cSharpType)
        {
            Type? underlyingType = Nullable.GetUnderlyingType(cSharpType) ?? cSharpType;

            if (underlyingType.IsEnum) return "VARCHAR(50)";

            return underlyingType switch
            {
                var t when t == typeof(int) => "INT",
                var t when t == typeof(long) => "BIGINT",
                var t when t == typeof(Guid) => "UUID",
                var t when t == typeof(decimal) => "DECIMAL",
                var t when t == typeof(float) => "FLOAT",
                var t when t == typeof(double) => "DOUBLE PRECISION",

                var t when t == typeof(string) => "VARCHAR(255)",

                var t when t == typeof(DateTime) => "TIMESTAMP",
                var t when t == typeof(DateTimeOffset) => "TIMESTAMP WITH TIME ZONE",

                var t when t == typeof(bool) => "BOOLEAN",

                _ => throw new NotSupportedException(
                    $"Data type {cSharpType.Name} is not supported by the ORM.")
            };
        }
    }
}
