using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerComplexTypeTests
{
    private Core.Faker _faker;
        
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        
        public string ZipCode { get; set; }
    }
        
    public class Person
    {
        public Person(string name)
        {
            Name = name;
        }
            
        public string Name { get; }
        public int Age { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
    }
        
    public class Order
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }
            
        public int OrderId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
    }
        
    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
        
    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldInitializeNestedObjects()
    {
        // Arrange & Act
        var result = _faker.Create<Person>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.Not.Null);
        Assert.That(result.Age, Is.Not.EqualTo(0));
        Assert.That(result.HomeAddress, Is.Not.Null);
        Assert.That(result.HomeAddress.Street, Is.Not.Null);
        Assert.That(result.HomeAddress.City, Is.Not.Null);
        Assert.That(result.WorkAddress, Is.Not.Null);
    }
        
    [Test]
    public void Create_ShouldHandleObjectGraph_WithCollections()
    {
        // Arrange & Act
        Order result = _faker.Create<Order>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OrderId, Is.Not.EqualTo(0));
        Assert.That(result.Customer, Is.Not.Null);
        Assert.That(result.Customer.Name, Is.Not.Null);
        Assert.That(result.Customer.Email, Is.Not.Null);
        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(0));
    }
}