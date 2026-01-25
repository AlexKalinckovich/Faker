using System.Reflection;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Abstraction.TypeCreators;

public class ClassTypeCreator<T> : ITypeCreator<T>
{

    private readonly Faker _faker;
    public ClassTypeCreator(Faker faker)
    {
        _faker = faker;
    }
    
    public T Create(in Type type, in GeneratorFactory factory, in GeneratorContext context)
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
            T parameterValue = _faker.CreateTypeInstance<T>(parameterType);
            parameterValues[i] = parameterValue;
        }

        return parameterValues;
    }
}