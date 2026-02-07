using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Generators.TypeCreators;

public class PrimitiveTypeCreator : ITypeCreator
{
    private readonly Type _type;
    private readonly GeneratorFactory _factory;
    private readonly GeneratorContext _context;

    public PrimitiveTypeCreator(in Type type, in GeneratorFactory factory, in GeneratorContext context)
    {
        _type = type;
        _factory = factory;
        _context = context;
    }

    
    public object? Create()
    {
        IValueGenerator generator = _factory.GetGeneratorForType(_type);
        
        object? result = GenerateTypeUsingContext(generator, _context);
        
        return HandleNullability(result);
    }

    private object? GenerateTypeUsingContext(in IValueGenerator generator, in GeneratorContext context)
    {
        AssertGeneratorCanGenerateType(_type,generator);

        object? result = generator.Generate(_type, context);
        
        return result;
    }

    private static void AssertGeneratorCanGenerateType(Type parameterType, IValueGenerator generator)
    {
        if (!generator.CanGenerate(parameterType))
        {
            throw new ArgumentException($"Cannot generate type {parameterType.Name}");
        }
    }

    private object? HandleNullability(in object? value)
    {
    
        if (value == null && !_type.IsNullableType())
        {
            throw new InvalidOperationException(
                $"Generator returned null for non-nullable type {_type.Name}. " +
                $"This likely means a NullableGeneratorDecorator was used for a non-nullable type.");
        }
    
        return value;
    }
    
}