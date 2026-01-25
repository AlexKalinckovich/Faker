using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Short;

public class UShortGenerator : IntegerGenerator<ushort>
{
    public UShortGenerator() : base(
        random => (ushort)random.Next(0, ushort.MaxValue + 1),
        ushort.MinValue, 
        ushort.MaxValue
    ) { }
}