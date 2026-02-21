using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Generators.TypeCreators.utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public class CollectionGenerator
{
    private readonly GeneratorContext _context;
    private readonly GeneratorFactory _factory;
    
    public CollectionGenerator(
        GeneratorContext context, 
        GeneratorFactory factory)
    {
        _context = context;
        _factory = factory;
    }
    
    public bool IsCollection(Type type)
    {
        Type[] interfaces = type.GetInterfaces();
        return type.IsArray || 
               interfaces.Any(i => i.IsGenericType && 
                                   i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }
    
    public Type? GetCollectionType(Type type)
    {
        Type[] interfaces = type.GetInterfaces();
        
        foreach (Type interfaceType in interfaces)
        {
            if (interfaceType.IsGenericType && 
                interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return interfaceType;
            }
        }
        
        return null;
    }
    
    public void PopulateCollectionInstance(object instance, Type type)
    {
        if (type.IsArray)
        {
            PopulateArray((Array)instance, type);
        }
        else
        {
            Type? collectionType = GetCollectionType(type);
            if (collectionType != null)
            {
                PopulateGenericCollection(instance, collectionType);
            }
        }
    }
    
    private void PopulateArray(Array array, Type arrayType)
    {
        Type? elementType = arrayType.GetElementType();
        if (elementType != null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                ITypeCreator gen = TypeCreatorsFactory.GetTypeCreatorForType(elementType, _factory, _context);
                object? value = gen.Create();
                if (value != null)
                {
                    array.SetValue(value, i);
                }
            }
        }
    }
    
    private void PopulateGenericCollection(object collectionInstance, Type collectionType)
    {
        MethodInfo? addMethod = collectionType.GetMethod("Add");
        if (addMethod != null)
        {
            Type genericArgument = collectionType.GetGenericArguments()[0];
            int valueCount = GetRandomCollectionSize();
            
            for (int i = 0; i < valueCount; i++)
            {
                ITypeCreator gen = TypeCreatorsFactory.GetTypeCreatorForType(genericArgument, _factory, _context);
                object? argumentValue = gen.Create();
                if (argumentValue != null)
                {
                    addMethod.Invoke(collectionInstance, new[] { argumentValue });
                }
            }
        }
    }
    
    public bool HasNonDefaultCollectionValue(object instance, PropertyInfo property)
    {
        Type propertyType = property.PropertyType;
        
        if (propertyType.IsArray)
        {
            Array? arr = (Array?)property.GetValue(instance);
            return arr is { Length: > 0 };
        }
        
        Type? collectionType = GetCollectionType(propertyType);
        if (collectionType != null)
        {
            var countProperty = collectionType.GetProperty("Count");
            if (countProperty != null)
            {
                object? collectionInstance = property.GetValue(instance);
                if (collectionInstance != null)
                {
                    object? count = countProperty.GetValue(collectionInstance);
                    return count != null && (int)count > 0;
                }
            }
        }
        
        return false;
    }
    
    public void PopulateCollectionProperty(object instance, PropertyInfo property)
    {
        Type propertyType = property.PropertyType;
        
        if (propertyType.IsArray)
        {
            PopulateArrayProperty(instance, property);
        }
        else
        {
            Type? collectionType = GetCollectionType(propertyType);
            if (collectionType != null)
            {
                PopulateGenericCollectionProperty(instance, property, collectionType);
            }
        }
    }
    
    private void PopulateArrayProperty(object instance, PropertyInfo property)
    {
        Array? arr = (Array?)property.GetValue(instance);
        if (arr == null) return;
        
        Type? elementType = property.PropertyType.GetElementType();
        if (elementType != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                ITypeCreator gen = TypeCreatorsFactory.GetTypeCreatorForType(elementType, _factory, _context);
                object? value = gen.Create();
                arr.SetValue(value, i);
            }
        }
    }
    
    private void PopulateGenericCollectionProperty(
        object instance, 
        PropertyInfo property, 
        Type collectionType)
    {
        object? collectionInstance = property.GetValue(instance);
        if (collectionInstance == null) return;
        
        MethodInfo? addMethod = collectionType.GetMethod("Add");
        if (addMethod != null)
        {
            Type genericArgument = collectionType.GetGenericArguments()[0];
            int valueCount = GetRandomCollectionSize();
            
            for (int i = 0; i < valueCount; i++)
            {
                ITypeCreator gen = TypeCreatorsFactory.GetTypeCreatorForType(genericArgument, _factory, _context);

                object? argumentValue = gen.Create();
                if (argumentValue != null)
                {
                    addMethod.Invoke(collectionInstance, new[] { argumentValue });
                }
            }
        }
    }
    
    private int GetRandomCollectionSize()
    {
        
        return _context.Random.Next(0, 101);
    }
}