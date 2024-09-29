namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IIdeComponentRenderers
{
    public Type BooleanPromptOrCancelRendererType { get; }
    public Type FileFormRendererType { get; }
    public Type DeleteFileFormRendererType { get; }
    public Type InputFileRendererType { get; }
    public IdeTreeViews IdeTreeViews { get; }
}
