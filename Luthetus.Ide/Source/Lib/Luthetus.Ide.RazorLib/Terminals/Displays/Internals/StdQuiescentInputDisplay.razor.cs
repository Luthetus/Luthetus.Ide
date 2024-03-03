using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

public partial class StdQuiescentInputDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public IntegratedTerminal IntegratedTerminal { get; set; } = null!;
    [Parameter, EditorRequired]
    public StdQuiescent StdQuiescent { get; set; } = null!;

    private ResourceUri ResourceUri { get; set; } = null!;
    private Guid Id { get; set; }
    private Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.NewKey();

    protected override void OnInitialized()
    {
        Id = Guid.NewGuid();
        ResourceUri = new(nameof(StdQuiescentInputDisplay) + '_' + Id);

        base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var textEditorModel = new TextEditorModel(ResourceUri, DateTime.UtcNow, "terminal_quiescent", string.Empty, new IntegratedTerminalDecorationMapper(), null);
            TextEditorService.ModelApi.RegisterCustom(textEditorModel);
        }

        return base.OnAfterRenderAsync(firstRender);
    }

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

    public void Dispose()
    {
        TextEditorService.ModelApi.Dispose(ResourceUri);
    }
}
