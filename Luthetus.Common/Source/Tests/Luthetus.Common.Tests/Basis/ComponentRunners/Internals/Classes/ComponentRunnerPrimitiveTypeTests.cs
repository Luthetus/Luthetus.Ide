using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerParameter"/>
/// </summary>
public class ComponentRunnerPrimitiveTypeTests
{
    /// <summary>
    /// <see cref="ComponentRunnerParameter(System.Reflection.ConstructorInfo?, Func{object?}?, object?, Type)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerParameter.ChosenConstructorInfo"/>
    /// <see cref="ComponentRunnerParameter.ConstructValueFunc"/>
    /// <see cref="ComponentRunnerParameter.Value"/>
    /// <see cref="ComponentRunnerParameter.Type"/>
    /// <see cref="ComponentRunnerParameter.ComponentRunnerParameterKind"/>
    /// </summary>
    [Fact]
    public void Constructor_WITH_PrimitiveType()
    {
        var samplePrimitiveType = 37;

        ConstructorInfo? constructorInfo = null;

        Func<object?>? constructValueFunc = () => default;

        var type = typeof(int);

        var componentRunnerParameterKind = ComponentRunnerParameterKind.Primitive;

        var componentRunnerParameter = new ComponentRunnerParameter(
            constructorInfo,
            constructValueFunc,
            samplePrimitiveType,
            type);

        Assert.True(constructorInfo == componentRunnerParameter.ChosenConstructorInfo);
        Assert.True(constructValueFunc == componentRunnerParameter.ConstructValueFunc);
        Assert.True(samplePrimitiveType == (int)(componentRunnerParameter.Value ?? 0));
        Assert.True(type == componentRunnerParameter.Type);
        Assert.True(componentRunnerParameterKind == componentRunnerParameter.ComponentRunnerParameterKind);
    }

    /// <summary>
    /// <see cref="ComponentRunnerParameter(System.Reflection.ConstructorInfo?, Func{object?}?, object?, Type)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerParameter.ChosenConstructorInfo"/>
    /// <see cref="ComponentRunnerParameter.ConstructValueFunc"/>
    /// <see cref="ComponentRunnerParameter.Value"/>
    /// <see cref="ComponentRunnerParameter.Type"/>
    /// <see cref="ComponentRunnerParameter.ComponentRunnerParameterKind"/>
    /// </summary>
    [Fact]
    public void Constructor_WITH_ComplexType()
    {
        var sampleComplexType = new SampleComplexType("John", "Doe");

        var constructorInfo = typeof(SampleComplexType).GetConstructor(
            new[] { typeof(string), typeof(string) });

        Func<object?>? constructValueFunc = null;

        var type = typeof(SampleComplexType);

        var componentRunnerParameterKind = ComponentRunnerParameterKind.Complex;

        Assert.NotNull(constructorInfo);

        var componentRunnerParameter = new ComponentRunnerParameter(
            constructorInfo,
            null,
            sampleComplexType,
            type);

        Assert.True(constructorInfo == componentRunnerParameter.ChosenConstructorInfo);
        Assert.True(constructValueFunc == componentRunnerParameter.ConstructValueFunc);
        Assert.True(sampleComplexType == componentRunnerParameter.Value);
        Assert.True(type == componentRunnerParameter.Type);
        Assert.True(componentRunnerParameterKind == componentRunnerParameter.ComponentRunnerParameterKind);
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
