using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public record InspectContextRecordEntry(
    ContextRecordKeyHeirarchy ContextRecordKeyHeirarchy,
    MeasuredHtmlElementDimensions TargetContextRecordMeasuredHtmlElementDimensions);
