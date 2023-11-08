using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerComplexType"/>
/// </summary>
public record ComponentRunnerComplexTypeTests
{
    /// <summary>
    /// <see cref="ComponentRunnerComplexType(System.Reflection.ConstructorInfo?, Func{object?}?, object?, System.Type)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerComplexType.ChosenConstructorInfo"/>
    /// <see cref="ComponentRunnerComplexType.ConstructValueFunc"/>
    /// <see cref="ComponentRunnerComplexType.Value"/>
    /// <see cref="ComponentRunnerComplexType.Type"/>
    /// <see cref="ComponentRunnerComplexType.ComponentRunnerTypeKind"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var sampleComplexType = new SampleComplexType("John", "Doe");

        var constructorInfo = typeof(SampleComplexType).GetConstructor(
            new[] { typeof(string), typeof(string) });

        Func<object?>? constructValueFunc = null;

        var type = typeof(SampleComplexType);

        var componentRunnerTypeKind = ComponentRunnerTypeKind.Complex;

        Assert.NotNull(constructorInfo);

        var componentRunnerComplexType = new ComponentRunnerComplexType(
            constructorInfo,
            null,
            sampleComplexType,
            type);

        Assert.True(constructorInfo == componentRunnerComplexType.ChosenConstructorInfo);
        Assert.True(constructValueFunc == componentRunnerComplexType.ConstructValueFunc);
        Assert.True(sampleComplexType == componentRunnerComplexType.Value);
        Assert.True(type == componentRunnerComplexType.Type);
        Assert.True(componentRunnerTypeKind == componentRunnerComplexType.ComponentRunnerTypeKind);
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
