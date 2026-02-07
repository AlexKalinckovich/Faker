
using Faker.Core.Exceptions;
using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerTypeCreatorFactoryTests
{
    private Core.Faker _faker;
        
    public struct TestStruct
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }
        
    public enum TestEnum
    {
        First,
        Second,
        Third
    }
        
    public interface ITestInterface
    {
        string Value { get; }
    }
        
    public abstract class AbstractClass
    {
        public int Id { get; set; }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldHandleValueTypes()
    {
        // Arrange & Act
        TestStruct result = _faker.Create<TestStruct>();
            
        // Assert
        Assert.That(result.Number, Is.Not.EqualTo(0));
        Assert.That(result.Text, Is.Not.Null.And.Not.Empty);
    }
        
    [Test]
    public void Create_ShouldHandleEnums()
    {
        // Arrange & Act
        TestEnum result = _faker.Create<TestEnum>();
            
        // Assert
        Assert.That(Enum.IsDefined(typeof(TestEnum), result), Is.True);
    }
        
    [Test]
    public void Create_ShouldThrowException_ForInterface()
    {

        // Assert
        Assert.Throws<FakerCreationException>(() => _faker.Create<ITestInterface>());
    }
        
    [Test]
    public void Create_ShouldThrowException_ForAbstractClass()
    {
            
        // Assert
        Assert.Throws<FakerCreationException>(() => _faker.Create<AbstractClass>());
    }
}