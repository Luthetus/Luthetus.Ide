using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Terminals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TerminalSession> TerminalSessionKey { get; set; }

    private Key<TerminalSession> _seenTerminalSessionKey;

    private TextEditorViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
    {
        IncludeHeaderHelperComponent = false,
        IncludeFooterHelperComponent = false,
        IncludeGutterComponent = false,
        ContextRecord = ContextFacts.TerminalContext,
        KeymapOverride = new TextEditorKeymapTerminal()
    };

    protected override void OnParametersSet()
    {
        if (_seenTerminalSessionKey != TerminalSessionKey)
        {
            _seenTerminalSessionKey = TerminalSessionKey;
            var terminalSessionState = TerminalSessionStateWrap.Value;

            if (terminalSessionState.TerminalSessionMap.TryGetValue(TerminalSessionKey, out var terminalSession))
            {
                var textEditorModel = TextEditorService.ModelApi.GetOrDefault(terminalSession.ResourceUri);

                if (textEditorModel is null)
                {
                    var model = new TextEditorModel(
                        terminalSession.ResourceUri,
                        DateTime.UtcNow,
                        "terminal",
                        string.Empty,
                        new TerminalDecorationMapper(),
                        null);

                    TextEditorService.ModelApi.RegisterCustom(model);

                    TextEditorService.ViewModelApi.Register(
                        terminalSession.TextEditorViewModelKey,
                        terminalSession.ResourceUri,
                        new TextEditorCategory("terminal"));
                }
            }
        }

        base.OnParametersSet();
    }
}