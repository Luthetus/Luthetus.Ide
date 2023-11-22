using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Reflectives.States;

public partial record ReflectiveState
{
    public record RegisterAction(ReflectiveModel Entry, int InsertionIndex);
    public record DisposeAction(Key<ReflectiveModel> Key);

    public record WithAction(
        Key<ReflectiveModel> Key,
        Func<ReflectiveModel, ReflectiveModel> WithFunc);
}