using Luthetus.Ide.ClassLib.Context;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);