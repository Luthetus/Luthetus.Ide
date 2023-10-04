using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

public record ComponentRunnerComplexType : IComponentRunnerType
{
    public ComponentRunnerComplexType(
        ConstructorInfo? chosenConstructorInfo,
        Func<object?>? constructValueFunc,
        object? value,
        Type type)
    {
        ChosenConstructorInfo = chosenConstructorInfo;
        ConstructValueFunc = constructValueFunc;
        Value = value;
        Type = type;
    }

    public ConstructorInfo? ChosenConstructorInfo { get; set; }
    public Func<object?>? ConstructValueFunc { get; set; }
    public object? Value { get; set; }

    public Type Type { get; set; }
    public ComponentRunnerTypeKind ComponentRunnerTypeKind => ComponentRunnerTypeKind.Complex;
}
