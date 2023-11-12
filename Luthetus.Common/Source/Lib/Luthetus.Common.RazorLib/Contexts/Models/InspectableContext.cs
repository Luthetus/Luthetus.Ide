using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

/// <summary>
/// This logic relates to rendering a blue overlay during the 'inspect element like' action.
/// Each InspectContextRecordEntry relates to a single ContextRecord.
/// </summary>
public record InspectableContext(
    ContextHeirarchy ContextHeirarchy,
    MeasuredHtmlElementDimensions TargetContextRecordMeasuredHtmlElementDimensions);
