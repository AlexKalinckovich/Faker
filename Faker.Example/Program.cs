using Faker.Core.Extensions.Type;

namespace Faker.Example;

class Sample
{
    
    public class Person
    {
        public string Name { get; }
        public int Age { get; }
    
        public Person(string name, int age) 
        {
            Name = name;
            Age = age;
        }
    
        public Person(string name) : this(name, 0) { }
    }


// Immutable objects work too
    public class Product
    {
        public string Name { get; }
        public decimal Price { get; }
        
        public Person Buyer { get; }
    
        public Product(string name, decimal price, Person buyer)
        {
            Name = name;
            Price = price;
            Buyer = buyer;
        }
    }
    public static void Main()
    {
        var faker = new Core.Faker();
        Product? person = faker.Create<Product>(); 
        
    }
}