using System.Collections.Concurrent;
using Faker.Core.Config;
using Faker.Core.Context;
using Faker.Core.Generators.Core.Abstraction;

namespace Faker.Example;

class Sample
{

    public class A
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public ConcurrentDictionary<string, int> Numbers { get;set; }
       
        public double[] Doubles { get; set; }
        
        public A()
        {
            
        }
        

        public override string ToString()
        {
            string numbersVal = Numbers.Aggregate("", (current, number) => current + number);
            return $"Name: {Name}, Surname: {Surname}, Numbers: {numbersVal}";
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
            return $"ToString C {A}";
        }
    }


    class NameGenerator : IValueGenerator
    {
        public object Generate(in Type typeToGenerate, in GeneratorContext context)
        {
            return "Name";
        }

        public bool CanGenerate(in Type type)
        {
            return type == typeof(string);
        }
    }
    
    public static void Main()
    {
        FakerConfig config = new FakerConfig();
        config.Add<A, string>(p => p.Name, new NameGenerator());
        
        var faker = new Core.Faker(config);
        A a = faker.Create<A>();
        
        Console.WriteLine(a.ToString());
    }
    
}