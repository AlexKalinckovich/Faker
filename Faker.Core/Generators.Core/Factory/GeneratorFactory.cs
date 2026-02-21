using System.Collections.Concurrent;
using System.Reflection;
using Faker.Core.Config;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Generators.Boolean;
using Faker.Core.Generators.Core.Generators.Byte;
using Faker.Core.Generators.Core.Generators.Char;
using Faker.Core.Generators.Core.Generators.DateTime;
using Faker.Core.Generators.Core.Generators.Decimal;
using Faker.Core.Generators.Core.Generators.Double;
using Faker.Core.Generators.Core.Generators.Enum;
using Faker.Core.Generators.Core.Generators.Float;
using Faker.Core.Generators.Core.Generators.Int;
using Faker.Core.Generators.Core.Generators.Long;
using Faker.Core.Generators.Core.Generators.Short;
using Faker.Core.Generators.Core.Generators.Special;
using Faker.Core.Generators.Core.Generators.String;

namespace Faker.Core.Generators.Core.Factory;

public class GeneratorFactory
{
    private const double DefaultNullProbability = 0.1;
    private readonly Dictionary<Type, IValueGenerator> _primitiveGenerators = new();
    private readonly FakerConfig _config;
    public GeneratorFactory()
    {
        RegisterPrimitiveGenerators();
        _config = new FakerConfig();
    }

    public GeneratorFactory(in FakerConfig config)
    {
        _config = config;
        KeyValuePair<Type, IValueGenerator>[] configGenerators = config.CustomGenerators.ToArray();

        RegisterPrimitiveGenerators();
        
        foreach (KeyValuePair<Type,IValueGenerator> configGenerator in configGenerators)
        {
            RegisterGenerator(configGenerator.Key, configGenerator.Value);
        }
    }

    public bool HasGeneratorForType(in Type type)
    {
        return type.IsEnum || _primitiveGenerators.ContainsKey(type);
    }
    
    private void RegisterPrimitiveGenerators()
    {
        
        RegisterGenerator(typeof(double), new DoubleGenerator());
        RegisterGenerator(typeof(float), new FloatGenerator());
        RegisterGenerator(typeof(decimal), new DecimalGenerator());
        
        RegisterGenerator(typeof(string), new StringGenerator());
        
        RegisterGenerator(typeof(int), new IntGenerator());
        RegisterGenerator(typeof(uint), new UIntGenerator());
        
        RegisterGenerator(typeof(short), new ShortGenerator());
        RegisterGenerator(typeof(ushort), new UShortGenerator());
        
        RegisterGenerator(typeof(long), new LongGenerator()); 
        RegisterGenerator(typeof(ulong), new ULongGenerator()); 
        
        RegisterGenerator(typeof(byte), new ByteGenerator());
        RegisterGenerator(typeof(sbyte), new SByteGenerator());
        
        RegisterGenerator(typeof(bool), new BooleanGenerator());
        
        RegisterGenerator(typeof(char), new CharGenerator()); 
        
        RegisterGenerator(typeof(Enum), new EnumGenerator());
        
        RegisterGenerator(typeof(DateTime), new DateTimeGenerator());
    }

    public IValueGenerator GetGeneratorForMember(MemberInfo member, Type memberType)
    {
        IValueGenerator? memberGen = _config.GetGeneratorForMember(member);
        if (memberGen != null)
        {
            Type underlyingType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                
            return WrapWithNullableDecoratorIfNeeded(underlyingType, memberType, memberGen);
        }
        
        return GetGeneratorForType(memberType);
    }
    
    private void RegisterGenerator(in Type type, in IValueGenerator generator)
    {
        _primitiveGenerators[type] = generator;
    }

    public IValueGenerator GetGeneratorForType(in Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        if (underlyingType.IsEnum)
        {
            return _primitiveGenerators[typeof(Enum)];
        }
        
        return GetGeneratorForUnderlyingType(underlyingType, type);
    }
    
    private IValueGenerator GetGeneratorForUnderlyingType(Type underlyingType, Type type)
    {
        return _primitiveGenerators.TryGetValue(underlyingType, out IValueGenerator? baseGenerator) ? 
            WrapWithNullableDecoratorIfNeeded(underlyingType, type, baseGenerator) : new ComplexTypeFallbackGenerator();
    }

    private static IValueGenerator WrapWithNullableDecoratorIfNeeded(Type underlyingType, Type type,
        IValueGenerator baseGenerator)
    {
        IValueGenerator generator = baseGenerator;
        if (type != underlyingType)
        {
            generator = new NullableGeneratorDecorator(baseGenerator, DefaultNullProbability);
        }
        
        
        return generator;
    }

    private class ComplexTypeFallbackGenerator : IValueGenerator
    {
        public bool CanGenerate(in Type type) => false;
    
        public object Generate(in Type typeToGenerate, in GeneratorContext context)
        {
            throw new NotSupportedException($"Complex type {typeToGenerate.Name} should be created via constructors");
        }
    }
}

