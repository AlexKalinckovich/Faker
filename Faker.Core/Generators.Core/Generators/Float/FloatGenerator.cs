using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Float;

public class FloatGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(float);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return (float)(context.Random.NextDouble() * 200 - 100);
    }
}