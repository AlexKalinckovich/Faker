using Faker.Core.Context;

namespace Faker.Core.Generators.Core.Abstraction.Generator;

public abstract class IntegerGenerator<T> : IValueGenerator where T : struct
{
    private readonly Func<Random, T> _generatorFunc;
    private readonly T _minValue;
    private readonly T _maxValue;

    protected IntegerGenerator(Func<Random, T> generatorFunc, T minValue, T maxValue)
    {
        _generatorFunc = generatorFunc;
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public bool CanGenerate(in Type type) => type == typeof(T);

    public object? Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return _generatorFunc(context.Random);
    }
}