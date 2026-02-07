using System;
using System.Diagnostics;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Generators.TypeCreators;

namespace Faker.Core.Generators.Core.Factory;

public static class TypeCreatorsFactory
{
    public static ITypeCreator GetTypeCreatorForType(in Type type, in GeneratorFactory factory, in GeneratorContext context)
    {
        return type.IsSimpleType() switch
        {
            true  => new PrimitiveTypeCreator(type, factory, context),
            false => new ClassTypeCreator(type, factory, context)
        };
    }
    
}