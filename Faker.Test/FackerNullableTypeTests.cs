using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerNullableTypeTests
{
    private Core.Faker _faker;
        
    public class TestClass
    {
        public DateTime? NullableDateTime { get; set; }
    }
    
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldHandleNullableInt()
    {
        // Arrange & Act
        int? result = _faker.Create<int?>();
            
        bool foundNull = false;
        bool foundValue = false;
            
        for (int i = 0; i < 100; i++)
        {
            int? testResult = _faker.Create<int?>();
            if (testResult.HasValue)
            {
                foundValue = true;
                Assert.That(testResult.Value, Is.Not.EqualTo(0));
            }
            else
            {
                foundNull = true;
            }
                
            if (foundNull && foundValue)
                break;
        }
            
        Assert.That(foundNull, Is.True, "Should have found null at least once");
        Assert.That(foundValue, Is.True, "Should have found non-null at least once");
    }
        
    [Test]
    public void Create_ShouldHandleNullableDateTime()
    {
        // Arrange & Act
        TestClass result = _faker.Create<TestClass>();
            
        // Assert - test multiple times
        bool foundNull = false;
        bool foundValue = false;
            
        for (int i = 0; i < 100; i++)
        {
            TestClass testResult = _faker.Create<TestClass>();
            if (testResult.NullableDateTime.HasValue)
            {
                foundValue = true;
                Assert.That(testResult.NullableDateTime.Value, Is.Not.EqualTo(default(DateTime)));
            }
            else
            {
                foundNull = true;
            }
                
            if (foundNull && foundValue)
                break;
        }
            
        Assert.That(foundNull, Is.True, "Should have found null at least once");
        Assert.That(foundValue, Is.True, "Should have found non-null at least once");
    }
}