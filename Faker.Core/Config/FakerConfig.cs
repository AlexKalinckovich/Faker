using System.Linq.Expressions;
using System.Reflection;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Config;

public readonly struct FakerConfig
{
    public readonly Dictionary<Type, IValueGenerator> CustomGenerators { get; } = new() ;

    public FakerConfig()
    {
        
    }

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