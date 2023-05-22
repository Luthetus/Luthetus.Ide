using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.Context;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);