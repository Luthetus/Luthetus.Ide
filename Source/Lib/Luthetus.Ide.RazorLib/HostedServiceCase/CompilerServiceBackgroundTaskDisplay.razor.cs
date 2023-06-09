using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.HostedServiceCase;

public partial class CompilerServiceBackgroundTaskDisplay : ComponentBase, ICompilerServiceBackgroundTaskDisplayRendererType
{
    [Parameter, EditorRequired]
    public IBackgroundTask BackgroundTask { get; set; } = null!;
}