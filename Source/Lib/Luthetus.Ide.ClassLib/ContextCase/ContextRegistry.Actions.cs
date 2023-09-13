using Luthetus.Ide.ClassLib.ContextCase;
using Luthetus.Ide.ClassLib.JavaScriptObjectsCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public partial record ContextRegistry
{
    public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);
    public record ToggleSelectInspectionTargetAction;
    public record SetSelectInspectionTargetTrueAction;
    public record SetSelectInspectionTargetFalseAction;
    public record SetInspectionTargetAction(ImmutableArray<ContextRecord>? ContextRecords);
    public record AddMeasuredHtmlElementDimensionsAction(ContextRecord ContextRecord, ImmutableArray<ContextRecord> ContextBoundaryHeirarchy, MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions);
}
