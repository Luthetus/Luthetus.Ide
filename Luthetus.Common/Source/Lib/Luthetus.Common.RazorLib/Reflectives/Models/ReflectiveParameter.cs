using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public class ReflectiveParameter : IReflectiveParameter
{
    public ReflectiveParameter(
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

    public ReflectiveParameterKind ReflectiveParameterKind =>
        Type.IsPrimitive || Type == typeof(string)
            ? ReflectiveParameterKind.Primitive
            : ReflectiveParameterKind.Complex;

    public static ReflectiveParameter ConstructString()
    {
        return new ReflectiveParameter(
            null,
            () => null,
            null,
            typeof(string));
    }

    public static ReflectiveParameter ConstructInt()
    {
        return new ReflectiveParameter(
            null,
            () => default,
            default,
            typeof(int));
    }

    public static ReflectiveParameter ConstructOther<VariableType>()
    {
        return new ReflectiveParameter(
            null,
            () => null,
            null,
            typeof(VariableType));
    }

    public static ReflectiveParameter ConstructOther(Type variableType)
    {
        return new ReflectiveParameter(
            null,
            () => null,
            null,
            variableType);
    }
}
