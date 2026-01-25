using Faker.Core.Context;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Abstraction;

public interface ITypeCreator<out T>
{
    public T Create(in Type type,in GeneratorFactory factory,in GeneratorContext context);
}