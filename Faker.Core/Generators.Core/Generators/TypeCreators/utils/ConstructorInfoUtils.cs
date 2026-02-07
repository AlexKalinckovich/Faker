using System.Reflection;

namespace Faker.Core.Generators.Core.Generators.TypeCreators.utils;

public static class ConstructorInfoUtils
{
    public static ConstructorInfo[] GetConstructorsOfTypeSortedByParameterCount(in Type type)
    {
        return type
            .GetConstructors(bindingAttr: BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending((ConstructorInfo c) => c.GetParameters().Length)
            .ToArray();
    }
}