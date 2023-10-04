using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

/// <summary>
/// Each PanelTab maintains its own element dimensions as
/// each panel might need different amounts of space to be functionally usable.
/// </summary>
public record PanelTab(
    Key<PanelTab> Key,
    ElementDimensions ElementDimensions,
    ElementDimensions BeingDraggedDimensions,
    Type ContentRendererType,
    Type IconRendererType,
    string DisplayName)
{
    public bool IsBeingDragged { get; set; }

    /// <summary>
    /// TODO: In progress feature: working on keymap that sets focus to a context record...
    /// ... and if the JavaScript set focus returns false (implying focus was NOT set) then
    /// perhaps the ContextRecord is tied to a PanelTab. If so, set the PanelTab as active
    /// then try again to set focus to the now rendered ContextRecord.
    /// </summary>
    public Key<ContextRecord>? ContextRecordKey { get; set; }
}