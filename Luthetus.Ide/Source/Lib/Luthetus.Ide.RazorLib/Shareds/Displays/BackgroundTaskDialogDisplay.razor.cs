using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class BackgroundTaskDialogDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IBackgroundTask[] SeenBackgroundTasks { get; set; } = null!;
}