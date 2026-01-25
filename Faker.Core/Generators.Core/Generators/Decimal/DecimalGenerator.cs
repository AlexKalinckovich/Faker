using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Decimal;

public class DecimalGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(decimal);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return (decimal)(context.Random.NextDouble() * 200 - 100);
    }
}