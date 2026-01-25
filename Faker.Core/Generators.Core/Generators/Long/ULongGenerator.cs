using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Long;

public class ULongGenerator : IValueGenerator
{
    public bool CanGenerate(in Type type) => type == typeof(ulong);

    public object Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        byte[] buffer = new byte[8];
        context.Random.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }
}