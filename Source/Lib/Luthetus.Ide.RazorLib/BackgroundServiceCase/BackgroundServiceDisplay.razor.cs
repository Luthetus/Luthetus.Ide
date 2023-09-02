using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.BackgroundServiceCase;

public partial class BackgroundServiceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IBackgroundTaskService BackgroundTaskQueue { get; set; } = null!;
}