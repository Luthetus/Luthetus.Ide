namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public class IdeComponentRenderers : IIdeComponentRenderers
{
    public IdeComponentRenderers(
        Type booleanPromptOrCancelRendererType,
        Type fileFormRendererType,
        Type deleteFileFormRendererType,
        Type inputFileRendererType,
        IdeTreeViews ideTreeViews)
    {
        BooleanPromptOrCancelRendererType = booleanPromptOrCancelRendererType;
        FileFormRendererType = fileFormRendererType;
        DeleteFileFormRendererType = deleteFileFormRendererType;
        InputFileRendererType = inputFileRendererType;
        IdeTreeViews = ideTreeViews;
    }

    public Type BooleanPromptOrCancelRendererType { get; }
    public Type FileFormRendererType { get; }
    public Type DeleteFileFormRendererType { get; }
    public Type InputFileRendererType { get; }
    public IdeTreeViews IdeTreeViews { get; }
}
