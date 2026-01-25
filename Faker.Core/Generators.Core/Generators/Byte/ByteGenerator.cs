using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Byte;

public class ByteGenerator : IntegerGenerator<byte>
{
    public ByteGenerator() : base(
        random => (byte)random.Next(byte.MinValue, byte.MaxValue + 1),
        byte.MinValue, 
        byte.MaxValue
    ) { }
}