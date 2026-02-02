using System;
using System.Reflection;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Factory;
using Faker.Core.Generators.Core.Validator;

namespace Faker.Core.Generators.Core.Abstraction.TypeCreators.utils;

public class ConstructorUtils
{
    private readonly ClassTypeCreator _classTypeCreator;

    public ConstructorUtils(ClassTypeCreator classTypeCreator)
    {
        _classTypeCreator = classTypeCreator;
    }

    public object CreateWithConstructorPreferenceByParameterCount(Type type)
    {
        ConstructorInfo[] constructors = ConstructorInfoUtils.GetConstructorsOfTypeSortedByParameterCount(type);
        
        
        foreach (ConstructorInfo constructor in constructors)
        {
            int constructorParameterCount = constructor.GetParameters().Length;
            
            if (constructorParameterCount > 0 && !CircularDependencyDetector.HasCircularDependency(constructor))
            {
                object? instance = TryCreateInstanceUsingProvidedConstructor(constructor);
                if (instance != null)
                {
                    return instance;
                }
            }
        }
        
        return CreateUsingDefaultConstructorOrThrow(type);
    }

    private object CreateUsingDefaultConstructorOrThrow(Type type)
    {
        try
        {
            
            return Activator.CreateInstance(type)!;
        }
        catch (MissingMethodException)
        {
            throw new InvalidOperationException(
                $"Circular dependency detected for type '{type.Name}', and no public parameterless constructor was found. " +
                "To resolve circular dependencies, ensure the class has a public no-arguments constructor.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create instance of '{type.Name}' using default constructor.", ex);
        }
    }
    
    private object? TryCreateInstanceUsingProvidedConstructor(in ConstructorInfo constructor)
    {
        try
        {
            return CreateInstanceViaConstructor(constructor);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private object CreateInstanceViaConstructor(in ConstructorInfo constructor)
    {
        ParameterInfo[] parameterInfos = constructor.GetParameters();
        object?[] parameterValues = CreateRandomConstructorParameters(parameterInfos);
        return constructor.Invoke(parameterValues);
    }

    private object?[] CreateRandomConstructorParameters(in ParameterInfo[] constructorParameterInfos)
    {
        object?[] constructorParameters = new object?[constructorParameterInfos.Length];

        for (int i = 0; i < constructorParameterInfos.Length; i++)
        {
            Type parameterType = constructorParameterInfos[i].ParameterType;

            object? constructorRandomParameter = _classTypeCreator.GenerateDependencyType(parameterType);

            constructorParameters[i] = constructorRandomParameter;
        }

        return constructorParameters;
    }
    
}