using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerClassCreationTests
{
    private Core.Faker _faker;
        
    public class SimpleClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
        
    public class ClassWithDefaultConstructorOnly
    {
        public ClassWithDefaultConstructorOnly()
        {
            Value = "default";
        }
            
        public string Value { get; set; }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldInitializeAllProperties_ForSimpleClass()
    {
        // Arrange & Act
        var result = _faker.Create<SimpleClass>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(0));
        Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
    }
        
    [Test]
    public void Create_ShouldUseDefaultConstructor_WhenNoParameters()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithDefaultConstructorOnly>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo("default"));
    }
}