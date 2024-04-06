using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Keymaps.Models.Terminals;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;

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
        KeymapOverride = new TextEditorKeymapTerminal()
    };
}