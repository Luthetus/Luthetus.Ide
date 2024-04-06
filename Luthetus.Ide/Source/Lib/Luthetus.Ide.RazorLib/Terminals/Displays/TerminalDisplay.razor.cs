using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Keymaps.Models.Terminals;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TerminalSession> TerminalSessionKey { get; set; }

    private TextEditorViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
    {
        IncludeHeaderHelperComponent = false,
        IncludeFooterHelperComponent = false,
        IncludeGutterComponent = false,
        ContextRecord = ContextFacts.TerminalContext,
    };

    private Key<TerminalSession> _seenTerminalSessionKey;

    protected override void OnParametersSet()
    {
        if (_seenTerminalSessionKey != TerminalSessionKey)
        {
            _seenTerminalSessionKey = TerminalSessionKey;

            _textEditorViewModelDisplayOptions = new()
            {
                IncludeHeaderHelperComponent = false,
                IncludeFooterHelperComponent = false,
                IncludeGutterComponent = false,
                ContextRecord = ContextFacts.TerminalContext,
                KeymapOverride = new TextEditorKeymapTerminal(TerminalSessionStateWrap, TerminalSessionKey)
            };
        }

        base.OnParametersSet();
    }
}