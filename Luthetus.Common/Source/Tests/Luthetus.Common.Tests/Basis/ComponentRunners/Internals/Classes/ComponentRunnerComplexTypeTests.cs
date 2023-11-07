using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerComplexType"/>
/// </summary>
public record ComponentRunnerComplexTypeTests
{
    /// <summary>
    /// <see cref="ComponentRunnerComplexType(System.Reflection.ConstructorInfo?, Func{object?}?, object?, System.Type)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var sampleComplexType = new SampleComplexType("John", "Doe");

        var constructor = typeof(SampleComplexType).GetConstructor(
            new[] { typeof(string), typeof(string) });

        Assert.NotNull(constructor);

        var componentRunnerComplexType = new ComponentRunnerComplexType(
            constructor,
            null,
            sampleComplexType,
            typeof(SampleComplexType));
    }

    /// <summary>
    /// <see cref="ComponentRunnerComplexType.ChosenConstructorInfo"/>
    /// </summary>
    [Fact]
    public void ChosenConstructorInfo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerComplexType.ConstructValueFunc"/>
    /// </summary>
    [Fact]
    public void ConstructValueFunc()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerComplexType.Value"/>
    /// </summary>
    [Fact]
    public void Value()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerComplexType.Type"/>
    /// </summary>
    [Fact]
    public void Type()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ComponentRunnerComplexType.ComponentRunnerTypeKind"/>
    /// </summary>
    [Fact]
    public void ComponentRunnerTypeKind()
    {
        throw new NotImplementedException();
    }

    public class SampleComplexType
    {
        public SampleComplexType(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DisplayName => $"{FirstName} {LastName}";
    }
}
