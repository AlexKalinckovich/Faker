namespace Faker.Core.Context;

public readonly struct GeneratorContext(in Random random, in Faker faker)
{
    public Random Random { get; } = random;

    public Faker Faker { get; } = faker;
    
}