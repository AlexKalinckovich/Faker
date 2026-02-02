using System;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Long;

public class LongGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(long);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        int high = context.Random.Next(int.MinValue, int.MaxValue);
        int low = context.Random.Next(int.MinValue, int.MaxValue);
        
        return ((long)high << 32) | (uint)low;
    }
}