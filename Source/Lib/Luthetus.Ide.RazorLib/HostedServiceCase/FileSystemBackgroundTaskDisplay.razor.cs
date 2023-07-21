namespace Luthetus.Ide.RazorLib.HostedServiceCase;

public partial class FileSystemBackgroundTaskDisplay : ComponentBase, IFileSystemBackgroundTaskDisplayRendererType
{
    [Parameter, EditorRequired]
    public IBackgroundTask BackgroundTask { get; set; } = null!;
}