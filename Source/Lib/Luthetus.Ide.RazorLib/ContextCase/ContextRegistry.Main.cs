using Fluxor;
using Luthetus.Ide.RazorLib.ContextCase;
using Luthetus.Ide.RazorLib.JavaScriptObjectsCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ContextCase;

[FeatureState]
public partial record ContextRegistry(
    ImmutableArray<ContextRecord> ActiveContextRecords,
    ImmutableArray<ContextRecord>? InspectionTargetContextRecords,
    bool IsSelectingInspectionTarget,
    ImmutableArray<(ContextRecord contextRecord, ImmutableArray<ContextRecord> contextBoundaryHeirarchy, MeasuredHtmlElementDimensions measuredHtmlElementDimensions)> MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples)
{
    private ContextRegistry() : this(ImmutableArray<ContextRecord>.Empty, null, false, ImmutableArray<(ContextRecord contextRecord, ImmutableArray<ContextRecord> contextBoundaryHeirarchy, MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty)
    {
        ActiveContextRecords = new[]
        {
            ContextFacts.GlobalContext
        }.ToImmutableArray();
    }
}