using System;
using System.Collections.Generic;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Config;

public readonly struct FakerConfig
{
    public Dictionary<Type, IValueGenerator> CustomGenerators { get; } = new() ;

    public FakerConfig(params KeyValuePair<Type, IValueGenerator>[] customGeneratorKeyValuePairs)
    {
        foreach (KeyValuePair<Type, IValueGenerator> customGenerator in customGeneratorKeyValuePairs)
        {
            AddGeneratorOfType(customGenerator.Key, customGenerator.Value);
        }
    }
    
    public void AddGeneratorOfType(Type type, in IValueGenerator generator)
    {
        CustomGenerators[type] = generator;
    }

    public IValueGenerator? GetGeneratorOfType(in Type type)
    {
        return CustomGenerators.GetValueOrDefault(type);
    }
}