using Faker.Core.Config;
using Faker.Core.Context;
using Faker.Core.Extensions.Type;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Generators.Boolean;
using Faker.Core.Generators.Core.Generators.Byte;
using Faker.Core.Generators.Core.Generators.Char;
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

    public GeneratorFactory()
    {
        RegisterPrimitiveGenerators();
    }

    public GeneratorFactory(in FakerConfig config)
    {
        RegisterPrimitiveGenerators();
        Dictionary<Type, IValueGenerator> configGenerators = config.CustomGenerators;
        foreach (KeyValuePair<Type,IValueGenerator> configGenerator in configGenerators)
        {
            RegisterGenerator(configGenerator.Key, configGenerator.Value);
        }
    }

    public bool HasGeneratorForType(in Type type)
    {
        return _primitiveGenerators.ContainsKey(type);
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
        
    }

    private void RegisterGenerator(in Type type, in IValueGenerator generator)
    {
        _primitiveGenerators[type] = generator;
    }

    public IValueGenerator GetGeneratorForType(in Type type)
    {
        if (type.IsSimpleType())
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (_primitiveGenerators.TryGetValue(underlyingType, out IValueGenerator? baseGenerator))
            {
                if (type.IsNullableType())
                {
                    return new NullableGeneratorDecorator(baseGenerator, DefaultNullProbability);
                }

                return baseGenerator;
            }
        }
        else
        {
            if (_primitiveGenerators.TryGetValue(type, out IValueGenerator? baseGenerator))
            {
                return baseGenerator;
            }
        }

        return new ComplexTypeFallbackGenerator();
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

