using System;
using System.Reflection;
using Faker.Core.Config;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Factory;
using Microsoft.VisualBasic.CompilerServices;

namespace Faker.Core;

public class Faker
{
    private readonly GeneratorFactory _generatorFactory;
    private readonly GeneratorContext _generatorContext;

    public Faker()
    {
        _generatorFactory = new GeneratorFactory();
        _generatorContext = new GeneratorContext(new Random(), this);
    }

    public Faker(FakerConfig config)
    {
        _generatorFactory = new GeneratorFactory(config);
        _generatorContext = new GeneratorContext(new Random(), this);
    }

    public T Create<T>()
    {
        Type type = typeof(T);
        return (T)CreateTypeInstance(type)!;
    }

    public object? CreateTypeInstance(in Type type)
    {
        ITypeCreator a = TypeCreatorsFactory.GetTypeCreatorForType(type, _generatorFactory, _generatorContext);
        return a.Create();
    }
    
    public IValueGenerator GetGeneratorForType(in Type type)
    {
        return _generatorFactory.GetGeneratorForType(type);
    }
}