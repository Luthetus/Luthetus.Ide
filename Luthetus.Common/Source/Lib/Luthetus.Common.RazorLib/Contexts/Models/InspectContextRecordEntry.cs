using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public record InspectContextRecordEntry(
    TargetContextRecordKeyAndHeirarchyBag TargetContextRecordKeyAndHeirarchyBag,
    MeasuredHtmlElementDimensions TargetContextRecordMeasuredHtmlElementDimensions);
