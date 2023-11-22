using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Models;

public interface IComponentRunnerParameter
{
    public ComponentRunnerParameterKind ComponentRunnerParameterKind { get; }
    public ConstructorInfo? ChosenConstructorInfo { get; set; }
    public object? Value { get; set; }
    public Type Type { get; set; }
}
