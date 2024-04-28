using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public interface IVirtualizationResultWithoutTypeMask
{
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }
    public TextEditorMeasurements TextEditorMeasurements { get; init; }
}