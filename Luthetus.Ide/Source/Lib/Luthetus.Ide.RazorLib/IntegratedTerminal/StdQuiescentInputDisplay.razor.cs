using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public partial class StdQuiescentInputDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IntegratedTerminal IntegratedTerminal { get; set; } = null!;
    [Parameter, EditorRequired]
    public StdQuiescent StdQuiescent { get; set; } = null!;

    public async Task HandleStdQuiescentOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (StdQuiescent.IsCompleted)
            return;

        var capturedTargetFilePath = StdQuiescent.TargetFilePath;
        var capturedArguments = StdQuiescent.Arguments;

        await IntegratedTerminal.HandleStdQuiescentOnKeyDown(
            keyboardEventArgs,
            StdQuiescent,
            capturedTargetFilePath,
            capturedArguments);
    }
}