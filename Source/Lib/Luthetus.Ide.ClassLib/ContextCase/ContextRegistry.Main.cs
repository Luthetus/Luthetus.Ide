using Fluxor;
using Luthetus.Ide.ClassLib.ContextCase;
using Luthetus.Ide.ClassLib.JavaScriptObjectsCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

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