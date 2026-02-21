using System.Reflection;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Factory;
using Faker.Core.Generators.Core.Generators.TypeCreators.utils;

namespace Faker.Core.Generators.Core.Generators.TypeCreators;


public class ClassTypeCreator : ITypeCreator
{
    private readonly Type _type;
    private readonly GeneratorFactory _factory;
    private readonly GeneratorContext _context;
    private readonly ConstructorUtils _constructorUtils;
    private readonly CollectionGenerator _collectionGenerator;
    
    private readonly Dictionary<Type, object> _createdInstances = new();

    public ClassTypeCreator(in Type type, in GeneratorFactory factory, in GeneratorContext context)
    {
        _type = type;
        _factory = factory;
        _context = context;
        _constructorUtils = new ConstructorUtils(this);
        _collectionGenerator = new CollectionGenerator(context, factory);
    }
    
    public object? Create()
    {
        return CreateClassType(_type);
    }

    public object? CreateClassType(in Type type)
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

        return GenerateNewClassInstance(type);
    }

    private object GenerateNewClassInstance(Type type)
    {
        object classInstance = _constructorUtils.CreateWithConstructorPreferenceByParameterCount(type);

        _createdInstances.TryAdd(type, classInstance);


        PopulateWithValuesIfTypeIsCollection(type, classInstance);
        
        InitializeAllProperties(classInstance);
        
        return classInstance;
    }

    private void PopulateWithValuesIfTypeIsCollection(Type type, object classInstance)
    {
        if (_collectionGenerator.IsCollection(type))
        {
            _collectionGenerator.PopulateCollectionInstance(classInstance, type);
        }
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
        
        
        if (_collectionGenerator.IsCollection(property.PropertyType))
        {
            _collectionGenerator.PopulateCollectionProperty(instance, property);
        }
    }

    private bool PropertyHasNonDefaultValue(in object instance, in PropertyInfo property)
    {
        int propertyParameterCount = property.GetIndexParameters().Length;
        if (propertyParameterCount > 0)
        {
            return false;
        }
        
        Type propertyType = property.PropertyType;
        
        
        if (_collectionGenerator.IsCollection(propertyType))
        {
            return _collectionGenerator.HasNonDefaultCollectionValue(instance, property);
        }
        
        object? currentValue = property.GetValue(instance);
        object? defaultValue = GetDefaultValueForType(property.PropertyType);
    
        return !Equals(currentValue, defaultValue);
    }

    private void SetPropertyWithRandomGeneratedValue(object instance, PropertyInfo property)
    {
        if (property.GetIndexParameters().Length == 0)
        {
            Type propertyType = property.PropertyType;
            if (_createdInstances.TryGetValue(propertyType, out var circularReference))
                property.SetValue(instance, circularReference);
            else
            {
                object? generatedValue = GenerateDependencyType(propertyType, property);
                property.SetValue(instance, generatedValue);
            }
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
        
        return generator.Create();
    }
    
    internal object? GenerateDependencyType(Type type, MemberInfo? member)
    {
        if (!_collectionGenerator.IsCollection(type) && type.IsSimpleType())
        {
            PrimitiveTypeCreator primitiveCreator = new PrimitiveTypeCreator(type, _factory, _context, member);
            
            return primitiveCreator.Create();
        }

        return CreateClassType(type);
    }


    private static PropertyInfo[] GetPublicInstanceProperties(in Type type)
    {
        PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    
        return allProperties
            .Where((PropertyInfo p) => p is { CanWrite: true, SetMethod.IsPublic: true })
            .ToArray();
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