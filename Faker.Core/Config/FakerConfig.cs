using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Config;

public class FakerConfig
{
    public ConcurrentDictionary<Type, IValueGenerator> CustomGenerators { get; } = new() ;
    private readonly ConcurrentDictionary<MemberInfo, IValueGenerator> _memberGenerators = new();
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

    public void Add<TClass, TProperty>(
        Expression<Func<TClass, TProperty>> memberExpression,
        IValueGenerator generator)
    {
        if (memberExpression.Body is MemberExpression memberExpr)
        {
            MemberInfo member = memberExpr.Member;
            _memberGenerators[member] = generator;
        }
        else
        {
            throw new ArgumentException("Expression must be a member access (property or field)", nameof(memberExpression));
        }
    }
    
    public IValueGenerator? GetGeneratorForMember(MemberInfo member)
    {
        return _memberGenerators.TryGetValue(member, out var gen) ? gen : null;
    }
    
    public IValueGenerator? GetGeneratorOfType(in Type type)
    {
        return CustomGenerators.GetValueOrDefault(type);
    }
}