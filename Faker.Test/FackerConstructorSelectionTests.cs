
using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerConstructorSelectionTests
{
    private Core.Faker _faker;
        
    public class ClassWithMultipleConstructors
    {
        public int ConstructorNumber { get; set; }
        public int Value1 { get; }
        public string Value2 { get; }
        public bool Value3 { get; }
            
        public ClassWithMultipleConstructors()
        {
            Value1 = 0;
            Value2 = "default";
            Value3 = false;
            ConstructorNumber = 0;
        }
            
        public ClassWithMultipleConstructors(int value1)
        {
            Value1 = value1;
            Value2 = "single";
            Value3 = false;
            ConstructorNumber = 1;
        }
            
        public ClassWithMultipleConstructors(int value1, string value2)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = false;
            ConstructorNumber = 2;
        }
            
        public ClassWithMultipleConstructors(int value1, string value2, bool value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            ConstructorNumber = 3;
        }
    }
        
    public class ClassWithExceptionConstructor
    {
        public int Value { get; }
            
        public ClassWithExceptionConstructor()
        {
            Value = 1;
        }
            
        public ClassWithExceptionConstructor(int value)
        {
            throw new InvalidOperationException("Constructor failed");
        }
            
        public ClassWithExceptionConstructor(int value1, int value2)
        {
            throw new ArgumentException("Another constructor failed");
        }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldUseConstructorWithMostParameters()
    {
        const int constructorWithMostParameters = 3;
        // Arrange & Act
        ClassWithMultipleConstructors result = _faker.Create<ClassWithMultipleConstructors>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ConstructorNumber, Is.EqualTo(constructorWithMostParameters));
    }
        
    [Test]
    public void Create_ShouldFallbackToNextConstructor_WhenMaxParamConstructorThrows()
    {
        // Arrange & Act
        ClassWithExceptionConstructor result = _faker.Create<ClassWithExceptionConstructor>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(1));
    }
}