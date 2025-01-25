using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

/// <summary>
/// Verify that 'TargetContextRecordMeasuredHtmlElementDimensions is not null'
/// to know whether this was constructed or default.
///
/// This logic relates to rendering a blue overlay during the 'inspect element like' action.
/// Each InspectContextRecordEntry relates to a single ContextRecord.
/// </summary>
public record struct InspectableContext(
    ContextHeirarchy ContextHeirarchy,
    MeasuredHtmlElementDimensions TargetContextRecordMeasuredHtmlElementDimensions);
