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
    private readonly GeneratorFactory _generatorFactory = new();
    private readonly Random _random = new();
    private readonly GeneratorContext _generatorContext;

    public Faker()
    {
        _generatorContext = new GeneratorContext(_random, this);
    }

    public T Create<T>()
    {
        Type type = typeof(T);
        return CreateTypeInstance<T>(type);
    }

    public T CreateTypeInstance<T>(in Type type)
    {
        return type.IsSimpleType() ? 
            CreatePrimitiveType<T>(type) : 
            CreateClassType<T>(type);
    }

    private T CreateClassType<T>(in Type type)
    {
        ConstructorInfo[] constructors = GetConstructorsByParameterCount(type);
        
        AssertAtLeastOneConstructorOfTypeExists(type, constructors);
        
        foreach (ConstructorInfo constructor in constructors)
        {
            T? possibleInstance = TryCreateInstanceUsingProvidedConstructor<T>(constructor);
            if (possibleInstance != null)
            {
                return possibleInstance;
            }
        }

        throw new InvalidOperationException($"Failed to create instance of {type.Name}. All constructors threw exceptions.");
    }
    
    private T CreatePrimitiveType<T>(in Type type)
    {
        IValueGenerator generator = GetGeneratorForType(type);
        
        object? result = GenerateTypeUsingContext<T>(generator);
        
        return HandleNullability<T>(result);
    }

    private object? GenerateTypeUsingContext<T>(in IValueGenerator generator)
    {
        AssertGeneratorCanGenerateType(typeof(T),generator);

        object? result = generator.Generate(typeof(T), _generatorContext);
        return result;
    }

    private static void AssertGeneratorCanGenerateType(Type parameterType, IValueGenerator generator)
    {
        if (!generator.CanGenerate(parameterType))
        {
            throw new ArgumentException($"Cannot generate type {parameterType.Name}");
        }
    }

    private T HandleNullability<T>(in object? value)
    {
        Type type = typeof(T);
    
        if (value == null && type.IsSimpleType() && Nullable.GetUnderlyingType(type) == null)
        {
            throw new InvalidOperationException(
                $"Generator returned null for non-nullable type {type.Name}. " +
                $"This likely means a NullableGeneratorDecorator was used for a non-nullable type.");
        }
    
        return (T)value!;
    }

    private T? TryCreateInstanceUsingProvidedConstructor<T>(in ConstructorInfo constructor)
    {
        T? possibleInstanceOfTypeT;
        try
        {
            possibleInstanceOfTypeT = InvokeConstructor<T>(constructor);
        }
        catch (Exception ex) when (ex is TargetInvocationException or ArgumentException or ArgumentNullException)
        {
            possibleInstanceOfTypeT = default;
        }
        
        return possibleInstanceOfTypeT;
    }

    private T InvokeConstructor<T>(in ConstructorInfo constructor)
    {
        ParameterInfo[] parameters = constructor.GetParameters();
        object?[] parameterValues = CreateParameterValues<T>(parameters);
        object instance = constructor.Invoke(parameterValues);
        return (T)instance;
    }

    private object?[] CreateParameterValues<T>(in ParameterInfo[] parameters)
    {
        object?[] parameterValues = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            T parameterValue = CreateTypeInstance<T>(parameterType);
            parameterValues[i] = parameterValue;
        }

        return parameterValues;
    }


    private static void AssertAtLeastOneConstructorOfTypeExists(in Type type, in ConstructorInfo[] constructors)
    {
        if (constructors.Length == 0)
        {
            throw new InvalidOperationException($"Type {type.Name} has no constructors available for creation");
        }
    }

    private ConstructorInfo[] GetConstructorsByParameterCount(Type type)
    {
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .ToArray();
    }

    private object? GenerateParameterValue(Type parameterType, GeneratorContext context)
    {
        IValueGenerator generator = _generatorFactory.GetGeneratorForType(parameterType);
        AssertGeneratorCanGenerateType(parameterType, generator);
        
        return generator.Generate(parameterType, context);
    }
    

    public T? Create<T>(in FakerConfig config)
    {
        return Create<T>();
    }

    public IValueGenerator GetGeneratorForType(in Type type)
    {
        return _generatorFactory.GetGeneratorForType(type);
    }
}