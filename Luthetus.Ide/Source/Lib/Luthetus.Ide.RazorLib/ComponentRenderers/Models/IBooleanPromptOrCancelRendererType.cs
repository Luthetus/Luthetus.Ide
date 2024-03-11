namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IBooleanPromptOrCancelRendererType
{
    public bool IncludeCancelOption { get; set; }
    public string Message { get; set; }
    public string? AcceptOptionTextOverride { get; set; }
    public string? DeclineOptionTextOverride { get; set; }
    public Func<Task> OnAfterAcceptFunc { get; set; }
    public Func<Task> OnAfterDeclineFunc { get; set; }
    public Func<Task> OnAfterCancelFunc { get; set; }
}