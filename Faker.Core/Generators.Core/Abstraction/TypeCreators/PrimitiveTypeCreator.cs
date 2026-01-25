using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Factory;

namespace Faker.Core.Generators.Core.Abstraction.TypeCreators;

public class PrimitiveTypeCreator<T> : ITypeCreator<T>
{
    public T Create(in Type type, in GeneratorFactory factory, in GeneratorContext context)
    {
        IValueGenerator generator = factory.GetGeneratorForType(type);
        
        object? result = GenerateTypeUsingContext<T>(generator, context);
        
        return HandleNullability<T>(result);
    }

    private object? GenerateTypeUsingContext<T>(in IValueGenerator generator, in GeneratorContext context)
    {
        AssertGeneratorCanGenerateType(typeof(T),generator);

        object? result = generator.Generate(typeof(T), context);
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
    
}