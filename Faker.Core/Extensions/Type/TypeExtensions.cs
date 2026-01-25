namespace Faker.Core.Extensions.Type;

public static class TypeExtensions
{
    public static bool IsNullableType(this System.Type type)
    {
        return Nullable.GetUnderlyingType(type) != null || !IsSimpleType(type);
    }

    public static System.Type GetUnderlyingTypeIfNullable(this System.Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    public static bool IsSimpleType(this System.Type type)
    {
        bool isSimpleType = type.IsPrimitive ||
                            type.IsEnum ||
                            type == typeof(string) ||
                            type == typeof(decimal);
        
        System.Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null && !isSimpleType)
        {
            isSimpleType = IsSimpleType(underlyingType);
        }
        
        return isSimpleType;
    }
    
}