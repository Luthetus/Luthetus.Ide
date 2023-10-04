using System.Reflection;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;

public interface IComponentRunnerType
{
    public ComponentRunnerTypeKind ComponentRunnerTypeKind { get; }
    public ConstructorInfo? ChosenConstructorInfo { get; set; }
    public object? Value { get; set; }
    public Type Type { get; set; }
}
