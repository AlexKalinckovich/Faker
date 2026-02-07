using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Core.Generators.Core.Generators.DateTime;

public class DateTimeGenerator : IValueGenerator
{
    private static readonly System.DateTime Start =  new System.DateTime(1970, 1, 1);
    
    private readonly int _range = (System.DateTime.Today - Start).Days;

    private System.DateTime Next(Random gen)
    {
        return Start.AddDays(gen.Next(_range))
                    .AddHours(gen.Next(0, 24))
                    .AddMinutes(gen.Next(0, 60))
                    .AddSeconds(gen.Next(0, 60));
    }

    public object Generate(in Type typeToGenerate, in GeneratorContext context)
    {
        Random random = context.Random;
        return Next(random);
    }

    public bool CanGenerate(in Type type)
    {
        return type == typeof(System.DateTime);
    }
}