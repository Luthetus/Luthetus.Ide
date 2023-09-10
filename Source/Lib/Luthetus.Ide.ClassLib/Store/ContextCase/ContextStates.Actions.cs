using Luthetus.Ide.ClassLib.Context;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public partial record ContextStates
{
    public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);
}
