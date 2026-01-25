using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.String;

public class StringGenerator : IValueGenerator
{
    private const uint MaxStringLength = 512;
    public bool CanGenerate(in Type type) => type == typeof(string);

    public object Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        uint length = GetRandomLengthUsingGeneratorInContext(context);
        if (length == 0)
            return string.Empty;
        
        return GenerateRandomStringUsingCharGeneratorFromContext(context, length);
    }

    private static object GenerateRandomStringUsingCharGeneratorFromContext(in GeneratorContext context, uint length)
    {
        IValueGenerator charGenerator = context.Faker.GetGeneratorForType(typeof(char));
        
        char[] characters = new char[length];
        for (int i = 0; i < length; i++)
        {
            object? charObj = charGenerator.Generate(typeof(char), context);
            characters[i] = (char)(charObj ?? throw new InvalidOperationException());
        }
        
        return new string(characters);
    }

    private static uint GetRandomLengthUsingGeneratorInContext(in GeneratorContext context)
    {
        IValueGenerator intGenerator = context.Faker.GetGeneratorForType(typeof(uint));
        
        object? lengthObj = intGenerator.Generate(typeof(uint), context);
        uint length = (uint)(lengthObj ?? throw new InvalidOperationException());
        
        length = Math.Min(length, MaxStringLength);
        
        return length;
    }
}