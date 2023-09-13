namespace Luthetus.Ide.RazorLib.ComponentRenderersCase;

public interface ILuthetusIdeComponentRenderers
{
    public Type? BooleanPromptOrCancelRendererType { get; }
    public Type? FileFormRendererType { get; }
    public Type? DeleteFileFormRendererType { get; }
    public Type? NuGetPackageManagerRendererType { get; }
    public Type? GitDisplayRendererType { get; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type? InputFileRendererType { get; }
    public LuthetusIdeTreeViews LuthetusIdeTreeViews { get; }
}
