using Faker.Core.Context;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Abstraction;

public interface ITypeCreator
{
    public object? Create();
}