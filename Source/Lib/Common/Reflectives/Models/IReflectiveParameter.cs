using System.Reflection;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public interface IReflectiveParameter
{
    public ReflectiveParameterKind ReflectiveParameterKind { get; }
    public ConstructorInfo? ChosenConstructorInfo { get; set; }
    public object? Value { get; set; }
    public Type Type { get; set; }
}
