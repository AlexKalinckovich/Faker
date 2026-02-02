using System;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Double;

public class DoubleGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(double);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return context.Random.NextDouble() * 200 - 100; 
    }
}