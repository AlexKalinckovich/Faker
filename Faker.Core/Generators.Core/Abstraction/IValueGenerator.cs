using System;
using Faker.Core.Context;

namespace Faker.Core.Generators.Core.Abstraction;

public interface IValueGenerator
{
    object? Generate(in Type typeToGenerate, in GeneratorContext context);
    
    bool CanGenerate(in Type type);
    
}