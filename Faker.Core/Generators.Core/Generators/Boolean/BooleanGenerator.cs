using System;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Boolean;

public class BooleanGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(bool);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return context.Random.NextDouble() < 0.5;
    }
}