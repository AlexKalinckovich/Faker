namespace Faker.Core.Extensions.Type;

public static class TypeExtensions
{
    public static bool IsNullableType(this System.Type type)
    {
        return !type.IsValueType || 
               Nullable.GetUnderlyingType(type) != null;
    }

    public static bool IsStandardLibraryType(this System.Type type)
    {
        
        return type.Namespace != null && 
               (type.Namespace.StartsWith("System") || 
                type.Namespace.StartsWith("Microsoft"));
    }

    public static bool CanProduceCircularDependency(this System.Type type)
    {
        return !type.IsSimpleType() && !type.IsStandardLibraryType();
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