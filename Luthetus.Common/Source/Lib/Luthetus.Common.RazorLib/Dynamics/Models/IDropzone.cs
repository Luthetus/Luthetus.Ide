using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDropzone
{
    public Key<IDropzone> DropzoneKey { get; }
    public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
    public ElementDimensions ElementDimensions { get; }
    public string CssClass { get; }
    public string CssStyle { get; }
}
