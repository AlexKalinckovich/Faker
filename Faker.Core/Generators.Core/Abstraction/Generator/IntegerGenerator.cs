using Faker.Core.Context;

namespace Faker.Core.Generators.Core.Abstraction.Generator;

public abstract class IntegerGenerator<T> : IValueGenerator where T : struct
{
    private readonly Func<Random, T> _generatorFunc;

    protected IntegerGenerator(Func<Random, T> generatorFunc)
    {
        _generatorFunc = generatorFunc;
    }

    public bool CanGenerate(in Type type) => type == typeof(T);

    public object Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        return _generatorFunc(context.Random);
    }
}