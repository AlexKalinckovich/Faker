using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Short;

public class ShortGenerator : IntegerGenerator<short>
{
    public ShortGenerator() : base(
        random => (short)random.Next(short.MinValue, short.MaxValue + 1)
    ) { }
}