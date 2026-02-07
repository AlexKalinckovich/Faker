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
                // Private setter cannot be tracked from outside
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
        // Arrange & Act
        ClassWithPrivateSetter result = _faker.Create<ClassWithPrivateSetter>();
            
        // Assert - Only public setters should be called
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PublicSetterCalled, Is.True, "Public setter should have been called");
        Assert.That(result.PrivateSetterCalled, Is.False, "Private setter should NOT have been called");
    }
        
    [Test]
    public void Create_ShouldHandleReadOnlyProperties_InitializedByConstructor()
    {
        // Arrange & Act
        ClassWithReadOnlyProperty result = _faker.Create<ClassWithReadOnlyProperty>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ConstructorCalled, Is.True, "Constructor should have been called");
        Assert.That(result.ConstructorValue, Is.Not.EqualTo(0), "Constructor value should be non-zero");
        Assert.That(result.ReadOnlyProperty, Is.Not.EqualTo(0), "Read-only property should be set by constructor");
        Assert.That(result.WritableProperty, Is.Not.EqualTo(0), "Writable property should be initialized");
    }
        
    [Test]
    public void Create_ShouldHandleCircularReferences_WithoutStackOverflow()
    {
        // Arrange & Act
        CircularReferenceClassA result = _faker.Create<CircularReferenceClassA>();
            
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.Not.EqualTo(0));
        Assert.That(result.ReferenceB, Is.Not.Null);
        Assert.That(result.ReferenceB.Name, Is.Not.Null.And.Not.Empty);
        Assert.That(result.ReferenceB.ReferenceA, Is.Not.Null);
        // Should be the same instance to avoid infinite recursion
        Assert.That(ReferenceEquals(result, result.ReferenceB.ReferenceA), Is.True);
    }
    
    [Test]
    public void Create_ShouldNotSetProperties_WithoutSetters()
    {
        // Arrange
        var faker = new Core.Faker();
        
        // Act
        var result = faker.Create<ClassWithoutSetters>();
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetterOnlyProperty, Is.EqualTo("Initial"));
    }
    
    public class ClassWithoutSetters
    {
        public string GetterOnlyProperty => "Initial";
        public int ComputedProperty => 42;
    }
}

[TestFixture]
public class FakerPropertyInitializationLogicTests
{
    private Core.Faker _faker;
    
    public class ClassWithPropertyInitializer
    {
        public string PreInitializedProperty { get; set; } = "PreInitialized";
        public int ComputedProperty => 100;
        public string ConstructorInitializedProperty { get; }
        public string DefaultProperty { get; set; }
        
        public ClassWithPropertyInitializer(string value)
        {
            ConstructorInitializedProperty = value;
        }
    }
    
    public class ClassWithDefaultValues
    {
        public int IntProperty { get; set; } = 999;
        public string StringProperty { get; set; } = "DefaultValue";
        public bool BoolProperty { get; set; } = true;
    }
    
    [SetUp]
    public void Setup()
    {
        _faker = new Core.Faker();
    }
    
    [Test]
    public void Create_ShouldNotOverride_PreInitializedProperties()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithPropertyInitializer>();
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PreInitializedProperty, Is.EqualTo("PreInitialized"), 
            "Property with initializer should not be overridden");
        Assert.That(result.ConstructorInitializedProperty, Is.Not.Null.And.Not.Empty,
            "Constructor-initialized property should have value");
        Assert.That(result.DefaultProperty, Is.Not.Null.And.Not.Empty,
            "Default property should be initialized by Faker");
        Assert.That(result.ComputedProperty, Is.EqualTo(100),
            "Computed property should not be changed");
    }
    
    [Test]
    public void Create_ShouldOverride_DefaultValues_WhenNotSetByConstructor()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithDefaultValues>();
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IntProperty, Is.Not.EqualTo(999), 
            "Default int value should be overridden");
        Assert.That(result.StringProperty, Is.Not.EqualTo("DefaultValue"),
            "Default string value should be overridden");
        Assert.That(result.BoolProperty, Is.Not.EqualTo(true),
            "Default bool value should be overridden (could be false)");
    }
    
    [Test]
    public void Create_ShouldDetect_ConstructorInitializedProperties()
    {
        // Arrange & Act
        var result = _faker.Create<ClassWithConstructorInitializationTracking>();
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ConstructorInitialized, Is.True,
            "Constructor-initialized property should be set by constructor");
        Assert.That(result.SetterInitialized, Is.True,
            "Other properties should be initialized by Faker");
    }
    
    public class ClassWithConstructorInitializationTracking
    {
        public bool ConstructorInitialized { get; }
        public bool SetterInitialized { get; set; }
        
        public ClassWithConstructorInitializationTracking()
        {
            ConstructorInitialized = true;
        }
    }
}