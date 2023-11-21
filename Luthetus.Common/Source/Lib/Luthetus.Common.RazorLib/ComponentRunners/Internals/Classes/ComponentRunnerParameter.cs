using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

public class ComponentRunnerParameter : IComponentRunnerParameter
{
    public ComponentRunnerParameter(
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

    public ComponentRunnerParameterKind ComponentRunnerParameterKind =>
        Type.IsPrimitive || Type == typeof(string)
            ? ComponentRunnerParameterKind.Primitive
            : ComponentRunnerParameterKind.Complex;

    public static ComponentRunnerParameter ConstructString()
    {
        return new ComponentRunnerParameter(
            null,
            () => null,
            null,
            typeof(string));
    }
    
    public static ComponentRunnerParameter ConstructInt()
    {
        return new ComponentRunnerParameter(
            null,
            () => default,
            default,
            typeof(int));
    }
    
    public static ComponentRunnerParameter ConstructOther<VariableType>()
    {
        return new ComponentRunnerParameter(
            null,
            () => null,
            null,
            typeof(VariableType));
    }
    
    public static ComponentRunnerParameter ConstructOther(Type variableType)
    {
        return new ComponentRunnerParameter(
            null,
            () => null,
            null,
            variableType);
    }
}
