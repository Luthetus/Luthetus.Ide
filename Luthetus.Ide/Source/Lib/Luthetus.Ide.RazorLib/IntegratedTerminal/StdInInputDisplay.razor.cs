using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public partial class StdInInputDisplay
{
    [Parameter, EditorRequired]
    public IntegratedTerminal IntegratedTerminal { get; set; } = null!;
    [Parameter, EditorRequired]
    public StdInRequest StdInRequest { get; set; } = null!;

    public async Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (StdInRequest.IsCompleted)
            return;

        var capturedValue = StdInRequest.Value;

        await IntegratedTerminal.HandleStdInputOnKeyDown(
            keyboardEventArgs,
            StdInRequest,
            capturedValue);
    }
}