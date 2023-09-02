using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Ide.RazorLib.BackgroundServiceCase;

public partial class BackgroundServicesPanelDisplay : ComponentBase
{
    [Inject]
    private ILuthetusCommonBackgroundTaskService LuthetusCommonBackgroundTaskService { get; set; } = null!;
    [Inject]
    private ILuthetusIdeFileSystemBackgroundTaskService LuthetusIdeFileSystemBackgroundTaskService { get; set; } = null!;
    [Inject]
    private ILuthetusIdeTerminalBackgroundTaskService LuthetusIdeTerminalBackgroundTaskService { get; set; } = null!;
    [Inject]
    private ILuthetusTextEditorCompilerServiceBackgroundTaskService LuthetusTextEditorCompilerServiceBackgroundTaskService { get; set; } = null!;
    [Inject]
    private ILuthetusTextEditorTextEditorBackgroundTaskService LuthetusTextEditorTextEditorBackgroundTaskService { get; set; } = null!;
}