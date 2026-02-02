using System;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Char;

public class CharGenerator : IValueGenerator
{
    private static readonly char[] CommonChars = 
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{}|;:,.<>? ".ToCharArray();

    public bool CanGenerate(in Type type) => type == typeof(char);

    public object Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return CommonChars[context.Random.Next(CommonChars.Length)];
    }
}