using System;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.Special;

public class NullableGeneratorDecorator : IValueGenerator
{
    private readonly IValueGenerator _innerGenerator;
    private readonly double _nullProbability;

    public NullableGeneratorDecorator(in IValueGenerator innerGenerator, double nullProbability)
    {
        _innerGenerator = innerGenerator ?? throw new ArgumentNullException(nameof(innerGenerator));
        _nullProbability = nullProbability;
    }

    public bool CanGenerate(in Type type)
    {
        return _innerGenerator.CanGenerate(type) || 
               IsNullableVersionOfSupportedType(type);
    }

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        
        if (typeToGenerate.IsNullableType() && IsNullProbabilityAchieved(context))
        {
            return GetNullValueForType(typeToGenerate);
        }
        
        return _innerGenerator.Generate(typeToGenerate, context);
    }

    private bool IsNullProbabilityAchieved(in GeneratorContext context)
    {
        return context.Random.NextDouble() < _nullProbability;
    }

    private bool IsNullableVersionOfSupportedType(in Type type)
    {
        Type? underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType != null && _innerGenerator.CanGenerate(underlyingType);
    }
    
    private object? GetNullValueForType(in Type type)
    {
        object? nullValue = null;
        if (type.IsValueType)
        {
            nullValue = GetNullValueForValueType(type);
        }
        return nullValue;
    }

    private static object? GetNullValueForValueType(in Type type)
    {
        AssertThatTypeIsNullableType(type);
        return Activator.CreateInstance(type);
    }

    private static void AssertThatTypeIsNullableType(in Type type)
    {
        Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is null)
        {
            throw new ArgumentException($"Cannot generate null for non-nullable value type {type.Name}");
        }
    }
}