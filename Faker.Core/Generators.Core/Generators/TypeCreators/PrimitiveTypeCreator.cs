using System.Reflection;
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
    private readonly MemberInfo? _member;

    public PrimitiveTypeCreator(Type type, GeneratorFactory factory, GeneratorContext context, MemberInfo? member = null)
    {
        _type = type;
        _factory = factory;
        _context = context;
        _member = member;
    }

    public object? Create()
    {
        IValueGenerator generator = _member != null
            ? _factory.GetGeneratorForMember(_member, _type)
            : _factory.GetGeneratorForType(_type);

        if (!generator.CanGenerate(_type))
            throw new ArgumentException($"Generator cannot produce type {_type}");

        object? value = generator.Generate(_type, _context);

        if (value == null && !_type.IsNullableType())
            throw new InvalidOperationException($"Generator returned null for non-nullable type {_type}");

        return value;
    }
}