namespace Luthetus.Ide.ClassLib.Panel;

/// <summary>
/// Each PanelTab maintains  its own element dimensions as
/// each panel tab might need different amounts of space to be functionally usable.
/// </summary>
public record PanelTab(
    PanelTabKey PanelTabKey,
    ElementDimensions ElementDimensions,
    ElementDimensions BeingDraggedDimensions,
    Type ContentRendererType,
    Type IconRendererType,
    string DisplayName)
{
    public bool IsBeingDragged { get; set; }
}