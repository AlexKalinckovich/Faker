using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerBasicTests
{
    private Core.Faker _faker;
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldReturnValue_ForPrimitiveType()
    {
        // Arrange & Act
        int result = _faker.Create<int>();
            
        // Assert
        Assert.That(result, Is.Not.EqualTo(0));
    }
        
    [Test]
    public void Create_ShouldReturnValue_ForStringType()
    {
        // Arrange & Act
        string result = _faker.Create<string>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
    }
        
    [Test]
    public void Create_ShouldReturnValue_ForDateTimeType()
    {
        // Arrange & Act
        DateTime result = _faker.Create<DateTime>();
            
        // Assert
        Assert.That(result, Is.Not.EqualTo(default(DateTime)));
    }
}