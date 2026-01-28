using System.Text;
using Faker.Core.Generators.Core.Validator;

namespace Faker.Example;

class Sample
{

    public class A
    {
        public A()
        {
        }

        public A A1 { get; set; }
        
        public A(A a)
        {
            A1 = a;
        }
        public override string ToString()
        {
            return $"To string A with value";
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
            return "ToString C";
        }
    }

    public static void Main()
    {
        var faker = new Core.Faker();
        StringBuilder a = faker.Create<StringBuilder>();
        Console.WriteLine(a.ToString());
    }
    
}