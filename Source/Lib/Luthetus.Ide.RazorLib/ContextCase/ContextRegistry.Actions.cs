using Luthetus.Ide.RazorLib.ContextCase;
using Luthetus.Ide.RazorLib.JavaScriptObjectsCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial record ContextRegistry
{
    public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);
    public record ToggleSelectInspectionTargetAction;
    public record SetSelectInspectionTargetTrueAction;
    public record SetSelectInspectionTargetFalseAction;
    public record SetInspectionTargetAction(ImmutableArray<ContextRecord>? ContextRecords);
    public record AddMeasuredHtmlElementDimensionsAction(ContextRecord ContextRecord, ImmutableArray<ContextRecord> ContextBoundaryHeirarchy, MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions);
}
