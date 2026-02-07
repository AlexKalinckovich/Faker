
using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerPropertyInitializationTests
{
    
    private Core.Faker _faker;
        
    public class ClassWithUninitializedProperties
    {
        public ClassWithUninitializedProperties(int id)
        {
            Id = id;
            // Name and IsActive are not initialized by constructor
        }
            
        public int Id { get; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
        
    public class ClassWithPartiallyInitializedProperties
    {
        public ClassWithPartiallyInitializedProperties(string name)
        {
            Name = name;
            // Id and IsActive are not initialized by constructor
        }
            
        public int Id { get; set; }
        public string Name { get; }
        public bool IsActive { get; set; }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldInitializeProperties_NotSetByConstructor()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithUninitializedProperties>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(0));
        Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
        Assert.That(result.IsActive, Is.Not.EqualTo(default(bool)));
    }
        
    [Test]
    public void Create_ShouldNotOverwriteProperties_SetByConstructor()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithPartiallyInitializedProperties>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Id, Is.Not.EqualTo(0));
        Assert.That(result.IsActive, Is.Not.EqualTo(default(bool)));
    }
}