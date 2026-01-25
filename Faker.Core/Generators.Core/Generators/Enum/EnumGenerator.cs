using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Enum;

public class EnumGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type.IsEnum;

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        AssertTypeIsEnum(typeToGenerate);

        Array enumValues = GetEnumValues(typeToGenerate);
        
        return GetRandomEnumValue(context.Random, enumValues);
    }

    private static object? GetRandomEnumValue(in Random random, in Array enumValues)
    {
        int randomIndex = random.Next(0, enumValues.Length);
        return enumValues.GetValue(randomIndex);
    }

    private static Array GetEnumValues(in Type typeToGenerate)
    {
        Array enumValues = System.Enum.GetValues(typeToGenerate);
        AssertValuesExistsInEnum(typeToGenerate, enumValues);
        return enumValues;
    }

    private static void AssertValuesExistsInEnum(in Type typeToGenerate, in Array enumValues)
    {
        if (enumValues.Length == 0)
            throw new ArgumentException($"Enum {typeToGenerate.Name} has no values");
    }

    private static void AssertTypeIsEnum(in Type typeToGenerate)
    {
        if (!typeToGenerate.IsEnum)
            throw new ArgumentException($"Type {typeToGenerate.Name} is not an enum");
    }
}