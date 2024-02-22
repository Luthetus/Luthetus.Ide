using Luthetus.Common.RazorLib.Reflectives.Models;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.Reflectives.Models;

/// <summary>
/// <see cref="ReflectiveParameter"/>
/// </summary>
public class ReflectiveParameterTests
{
    /// <summary>
    /// <see cref="ReflectiveParameter(ConstructorInfo?, Func{object?}?, object?, Type)"/>
    /// <br/>----<br/>
    /// <see cref="ReflectiveParameter.ChosenConstructorInfo"/>
    /// <see cref="ReflectiveParameter.ConstructValueFunc"/>
    /// <see cref="ReflectiveParameter.Value"/>
    /// <see cref="ReflectiveParameter.Type"/>
    /// <see cref="ReflectiveParameter.ReflectiveParameterKind"/>
    /// </summary>
    [Fact]
    public void Constructor_WITH_PrimitiveType()
    {
        var samplePrimitiveType = 37;

        ConstructorInfo? constructorInfo = null;

        Func<object?>? constructValueFunc = () => default;

        var type = typeof(int);

        var reflectiveParameterKind = ReflectiveParameterKind.Primitive;

        var reflectiveParameter = new ReflectiveParameter(
            constructorInfo,
            constructValueFunc,
            samplePrimitiveType,
            type);

        Assert.True(constructorInfo == reflectiveParameter.ChosenConstructorInfo);
        Assert.True(constructValueFunc == reflectiveParameter.ConstructValueFunc);
        Assert.True(samplePrimitiveType == (int)(reflectiveParameter.Value ?? 0));
        Assert.True(type == reflectiveParameter.Type);
        Assert.True(reflectiveParameterKind == reflectiveParameter.ReflectiveParameterKind);
    }

    /// <summary>
    /// <see cref="ReflectiveParameter(System.Reflection.ConstructorInfo?, Func{object?}?, object?, Type)"/>
    /// <br/>----<br/>
    /// <see cref="ReflectiveParameter.ChosenConstructorInfo"/>
    /// <see cref="ReflectiveParameter.ConstructValueFunc"/>
    /// <see cref="ReflectiveParameter.Value"/>
    /// <see cref="ReflectiveParameter.Type"/>
    /// <see cref="ReflectiveParameter.ReflectiveParameterKind"/>
    /// </summary>
    [Fact]
    public void Constructor_WITH_ComplexType()
    {
        var sampleComplexType = new SampleComplexType("John", "Doe");

        var constructorInfo = typeof(SampleComplexType).GetConstructor(
            new[] { typeof(string), typeof(string) });

        Func<object?>? constructValueFunc = null;

        var type = typeof(SampleComplexType);

        var reflectiveParameterKind = ReflectiveParameterKind.Complex;

        Assert.NotNull(constructorInfo);

        var reflectiveParameter = new ReflectiveParameter(
            constructorInfo,
            null,
            sampleComplexType,
            type);

        Assert.True(constructorInfo == reflectiveParameter.ChosenConstructorInfo);
        Assert.True(constructValueFunc == reflectiveParameter.ConstructValueFunc);
        Assert.True(sampleComplexType == reflectiveParameter.Value);
        Assert.True(type == reflectiveParameter.Type);
        Assert.True(reflectiveParameterKind == reflectiveParameter.ReflectiveParameterKind);
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
