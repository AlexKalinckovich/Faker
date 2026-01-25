using System.Diagnostics;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Abstraction.TypeCreators;

namespace Faker.Core.Generators.Core.Factory;

public static class TypeCreatorsFactory
{
    public static ITypeCreator<T> GetTypeCreatorForType<T>(in Type type, in Faker faker)
    {
        return type.IsSimpleType() switch
        {
            true  => new PrimitiveTypeCreator<T>(),
            false => new ClassTypeCreator<T>(faker)
        };
    }
    
}