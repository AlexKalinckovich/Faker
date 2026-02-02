using Faker.Core.Generators.Core.Abstraction.Generator;

namespace Faker.Core.Generators.Core.Generators.Int;

public class IntGenerator() : IntegerGenerator<int>(
    random => random.Next(-1000, 1001)
);









