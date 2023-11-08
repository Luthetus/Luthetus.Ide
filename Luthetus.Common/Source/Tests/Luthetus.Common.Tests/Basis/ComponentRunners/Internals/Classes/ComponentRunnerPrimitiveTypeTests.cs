using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using System.Reflection;

namespace Luthetus.Common.Tests.Basis.ComponentRunners.Internals.Classes;

/// <summary>
/// <see cref="ComponentRunnerPrimitiveType"/>
/// </summary>
public class ComponentRunnerPrimitiveTypeTests
{
    /// <summary>
    /// <see cref="ComponentRunnerPrimitiveType(System.Reflection.ConstructorInfo?, Func{object?}?, object?, Type)"/>
    /// <br/>----<br/>
    /// <see cref="ComponentRunnerPrimitiveType.ChosenConstructorInfo"/>
    /// <see cref="ComponentRunnerPrimitiveType.ConstructValueFunc"/>
    /// <see cref="ComponentRunnerPrimitiveType.Value"/>
    /// <see cref="ComponentRunnerPrimitiveType.Type"/>
    /// <see cref="ComponentRunnerPrimitiveType.ComponentRunnerTypeKind"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var samplePrimitiveType = 37;

        ConstructorInfo? constructorInfo = null;

        Func<object?>? constructValueFunc = () => default;

        var type = typeof(int);

        var componentRunnerTypeKind = ComponentRunnerTypeKind.Primitive;

        var componentRunnerComplexType = new ComponentRunnerPrimitiveType(
            constructorInfo,
            constructValueFunc,
            samplePrimitiveType,
            type);

        Assert.True(constructorInfo == componentRunnerComplexType.ChosenConstructorInfo);
        Assert.True(constructValueFunc == componentRunnerComplexType.ConstructValueFunc);
        Assert.True(samplePrimitiveType == (int)(componentRunnerComplexType.Value ?? 0));
        Assert.True(type == componentRunnerComplexType.Type);
        Assert.True(componentRunnerTypeKind == componentRunnerComplexType.ComponentRunnerTypeKind);
    }
}
