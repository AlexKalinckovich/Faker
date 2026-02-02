using System;
using System.Collections.Generic;
using System.Reflection;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction.TypeCreators.utils;
using Faker.Core.Generators.Core.Factory;
using Faker.Core.Generators.Core.Validator;

namespace Faker.Core.Generators.Core.Abstraction.TypeCreators;


public class ClassTypeCreator : ITypeCreator
{
    private readonly Type _type;
    private readonly GeneratorFactory _factory;
    private readonly GeneratorContext _context;
    private readonly ConstructorUtils _constructorUtils;
    
    private readonly Dictionary<Type, object> _createdInstances = new();

    public ClassTypeCreator(in Type type, in GeneratorFactory factory, in GeneratorContext context)
    {
        _type = type;
        _factory = factory;
        _context = context;
        _constructorUtils = new ConstructorUtils(this);
    }
    
    public object? Create()
    {
        return CreateClassType(_type);
    }

    private object? CreateClassType(in Type type)
    {
        if (_factory.HasGeneratorForType(type))
        {
            IValueGenerator generator = _factory.GetGeneratorForType(type);
            
            return generator.Generate(type, _context);
        }

        return GenerateClassInstance(type);
    }

    private object GenerateClassInstance(Type type)
    {
        if (_createdInstances.TryGetValue(type, out object? existing))
        {
            return existing;
        }
        
        object classInstance = _constructorUtils.CreateWithConstructorPreferenceByParameterCount(type);
        
        _createdInstances.TryAdd(type, classInstance);
        
        InitializeAllProperties(classInstance);
        
        return classInstance;
    }


    private void InitializeAllProperties(object classInstance)
    {
        PropertyInfo[] properties = GetPublicInstanceProperties(classInstance.GetType());
    
        foreach (PropertyInfo property in properties)
        {
            InitializePropertyIfNotSet(classInstance, property);
        }
    }

    private void InitializePropertyIfNotSet(in object instance, in PropertyInfo property)
    {
        if (!property.CanWrite || PropertyHasNonDefaultValue(instance, property))
            return;
        
        SetPropertyWithRandomGeneratedValue(instance, property);
    }

    private bool PropertyHasNonDefaultValue(in object instance, in PropertyInfo property)
    {
        object? currentValue = property.GetValue(instance);
        object? defaultValue = GetDefaultValueForType(property.PropertyType);
    
        return !Equals(currentValue, defaultValue);
    }

    private void SetPropertyWithRandomGeneratedValue(in object instance, in PropertyInfo property)
    {
        Type propertyType = property.PropertyType;
        if (_createdInstances.TryGetValue(propertyType, out object? circularReference))
        {
            property.SetValue(instance, circularReference);
        }
        else
        {
            object? generatedValue = GenerateDependencyType(propertyType);
            property.SetValue(instance, generatedValue);
        }
    }

    internal object? GenerateDependencyType(Type propertyType)
    {
        object? generatedValue = ShouldUseGeneratorFactory(propertyType) ? 
            GeneratePrimitiveOrSystemTypeUsingGenerators(propertyType) : 
            CreateClassType(propertyType);
        
        return generatedValue;
    }

    private object? GeneratePrimitiveOrSystemTypeUsingGenerators(Type propertyType)
    {
        ITypeCreator generator = TypeCreatorsFactory.GetTypeCreatorForType(
            propertyType, _factory, _context);
        return generator.Create();;
    }

    private static PropertyInfo[] GetPublicInstanceProperties(in Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    private static object? GetDefaultValueForType(in Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
    
    
    private bool ShouldUseGeneratorFactory(Type type)
    {
        return type.IsSimpleType() || type.IsStandardLibraryType();
    }
    
}