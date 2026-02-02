using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Int;

public class UIntGenerator : IntegerGenerator<uint>
{
    public UIntGenerator() : base(
        random => (uint)random.Next(0, 1001)
    ) { }
}