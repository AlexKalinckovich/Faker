using System.Text;
using Faker.Core.Config;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;
using Faker.Core.Generators.Core.Validator;

namespace Faker.Example;

class Sample
{

    public class A
    {
        public A()
        {
        }

        public string StringProperty { get; set; }
        
        public A A1 { get; set; }
        
        public A(A a)
        {
            A1 = a;
        }
        public override string ToString()
        {
            return $"To string A with value {StringProperty}";
        }
    }

    public class B
    {
        public B()
        {
        }
        public A A { get; set; }
        public C C { get; set; }
        
        
        public B(A a, C c)
        {
            A = a;
            C = c;
        }

        public override string ToString()
        {
            return $"ToString B called ToString C: {C} and C value: {C.Str}";
        }
    }
    
    public class C
    {
        public C()
        {
            
        }
        public A A { get; set; }

        public C(A a)
        {
            A = a;
        }

        public string Str => "C string";
        public override string ToString()
        {
            return $"ToString C {A.StringProperty}";
        }
    }
    
    private class CustomAGenerator : IValueGenerator
    {
        public object? Generate(in Type typeToGenerate, in GeneratorContext context)
        {
            return new A()
            {
                StringProperty = "Hello World!",
            };
        }

        public bool CanGenerate(in Type type)
        {
            return type == typeof(A);
        }
    }

    public static void Main()
    {
        FakerConfig config = new FakerConfig(new KeyValuePair<Type, IValueGenerator>(typeof(A), new CustomAGenerator()));
        
        var faker = new Core.Faker(config);
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(faker.Create<B>());
        }
    }
    
}