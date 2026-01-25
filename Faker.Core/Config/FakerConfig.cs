using System.Linq.Expressions;
using System.Reflection;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Config;

public class FakerConfig
{
    private readonly Dictionary<Type, Dictionary<string, IValueGenerator>> _fieldGenerators = new();
    
    private readonly Dictionary<Type, Dictionary<string, IValueGenerator>> _constructorParamGenerators = new();

    public void Add<TObject, TProperty, TGenerator>(in Expression<Func<TObject, TProperty>> propertySelector)
        where TGenerator : IValueGenerator, new()
    {
        var propertyInfo = GetPropertyInfo(propertySelector);
        var generator = new TGenerator();
        
        AddGenerator(typeof(TObject), propertyInfo.Name, generator, propertyInfo);
    }

    private PropertyInfo GetPropertyInfo<TObject, TProperty>(Expression<Func<TObject, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member as PropertyInfo;
        }
        throw new ArgumentException("Expression must be a property selector");
    }

    private void AddGenerator(Type objectType, string memberName, IValueGenerator generator, PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanWrite)
        {
            AddConstructorParameterGenerator(objectType, memberName, generator);
        }
        else
        {
            AddFieldGenerator(objectType, memberName, generator);
        }
    }

    private void AddFieldGenerator(in Type objectType, 
                                   in string fieldName, 
                                   in IValueGenerator generator)
    {
        if (!_fieldGenerators.ContainsKey(objectType))
            _fieldGenerators[objectType] = new Dictionary<string, IValueGenerator>();
        
        _fieldGenerators[objectType][fieldName] = generator;
    }

    private void AddConstructorParameterGenerator(Type objectType, string paramName, IValueGenerator generator)
    {
        if (!_constructorParamGenerators.ContainsKey(objectType))
            _constructorParamGenerators[objectType] = new Dictionary<string, IValueGenerator>();
        
        _constructorParamGenerators[objectType][paramName] = generator;
    }
}