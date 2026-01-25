using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Byte;

public class SByteGenerator : IntegerGenerator<sbyte>
{
    public SByteGenerator() : base(
        random => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue + 1),
        sbyte.MinValue, 
        sbyte.MaxValue
    ) { }
}