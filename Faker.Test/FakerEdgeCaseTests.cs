using NUnit.Framework;

namespace Faker.Test;

[TestFixture]
public class FakerEdgeCaseTests
{
    private Core.Faker _faker;
        
    public class ClassWithPrivateSetter
    {
        public bool PublicSetterCalled { get; private set; }
        public bool PrivateSetterCalled { get; private set; }
        
        private int _publicProperty;
        private string _privateSetterProperty;
        private bool _publicSetterProperty;
        
        public int PublicProperty 
        { 
            get => _publicProperty;
            private set
            {
                _publicProperty = value;
                
            }
        }
        
        public string PrivateSetterProperty 
        { 
            get => _privateSetterProperty;
            private set
            {
                _privateSetterProperty = value;
                PrivateSetterCalled = true;
            }
        }
        
        public bool PublicSetterProperty 
        { 
            get => _publicSetterProperty;
            set
            {
                _publicSetterProperty = value;
                PublicSetterCalled = true;
            }
        }
    }
        
    public class ClassWithReadOnlyProperty
    {
        public bool ConstructorCalled { get; }
        public int ConstructorValue { get; }
        
        public ClassWithReadOnlyProperty(int value)
        {
            ConstructorCalled = true;
            ConstructorValue = value;
            ReadOnlyProperty = value;
        }
            
        public int ReadOnlyProperty { get; }
        public int WritableProperty { get; set; }
    }
        
    public class CircularReferenceClassA
    {
        public CircularReferenceClassB ReferenceB { get; set; }
        public int Value { get; set; }
    }
        
    public class CircularReferenceClassB
    {
        public CircularReferenceClassA ReferenceA { get; set; }
        public string Name { get; set; }
    }
        
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
        
    [Test]
    public void Create_ShouldSetProperties_WithPublicSettersOnly()
    {
        
        ClassWithPrivateSetter result = _faker.Create<ClassWithPrivateSetter>();
            
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PublicSetterCalled, Is.True, "Public setter should have been called");
        Assert.That(result.PrivateSetterCalled, Is.False, "Private setter should NOT have been called");
    }
        
    [Test]
    public void Create_ShouldHandleReadOnlyProperties_InitializedByConstructor()
    {
        
        ClassWithReadOnlyProperty result = _faker.Create<ClassWithReadOnlyProperty>();
            
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ConstructorCalled, Is.True, "Constructor should have been called");
        Assert.That(result.ConstructorValue, Is.Not.EqualTo(0), "Constructor value should be non-zero");
        Assert.That(result.ReadOnlyProperty, Is.Not.EqualTo(0), "Read-only property should be set by constructor");
        Assert.That(result.WritableProperty, Is.Not.EqualTo(0), "Writable property should be initialized");
    }
        
    [Test]
    public void Create_ShouldHandleCircularReferences_WithoutStackOverflow()
    {
        
        CircularReferenceClassA result = _faker.Create<CircularReferenceClassA>();
            
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.Not.EqualTo(0));
        Assert.That(result.ReferenceB, Is.Not.Null);
        Assert.That(result.ReferenceB.Name, Is.Not.Null.And.Not.Empty);
        Assert.That(result.ReferenceB.ReferenceA, Is.Not.Null);
        
        Assert.That(ReferenceEquals(result, result.ReferenceB.ReferenceA), Is.True);
    }
    
    [Test]
    public void Create_ShouldNotSetProperties_WithoutSetters()
    {
        
        var faker = new Core.Faker();
        
        
        var result = faker.Create<ClassWithoutSetters>();
        
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetterOnlyProperty, Is.EqualTo("Initial"));
    }
    
    public class ClassWithoutSetters
    {
        public string GetterOnlyProperty => "Initial";
        public int ComputedProperty => 42;
    }
}
