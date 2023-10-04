using Luthetus.Common.RazorLib.ComponentRunners.Internals.Classes;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.ComponentRunners.States;

public partial record ComponentRunnerState
{
    public record RegisterAction(ComponentRunnerDisplayState Entry, int InsertionIndex);
    public record DisposeAction(Key<ComponentRunnerDisplayState> Key);

    public record WithAction(
        Key<ComponentRunnerDisplayState> Key,
        Func<ComponentRunnerDisplayState, ComponentRunnerDisplayState> WithFunc);
}