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
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;

    /// <summary>
    /// All the output from the terminal session will be displayed in a read-only text editor.
    /// </summary>
    [Parameter, EditorRequired]
    public Key<Terminal> TerminalKey { get; set; }
    
    /// <summary>
    /// Filter the output from a terminal session such that,
    /// only the output of the specified terminal command will be shown.
    /// </summary>
    [Parameter]
    public Key<TerminalCommand> TerminalCommandKey { get; set; } = Key<TerminalCommand>.Empty;

    private TextEditorViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
    {
        IncludeHeaderHelperComponent = false,
        IncludeFooterHelperComponent = false,
        IncludeGutterComponent = false,
        ContextRecord = ContextFacts.TerminalContext,
    };

    private Key<Terminal> _seenTerminalKey;

    protected override void OnParametersSet()
    {
        if (_seenTerminalKey != TerminalKey)
        {
            _seenTerminalKey = TerminalKey;

            _textEditorViewModelDisplayOptions = new()
            {
                IncludeHeaderHelperComponent = false,
                IncludeFooterHelperComponent = false,
                IncludeGutterComponent = false,
                ContextRecord = ContextFacts.TerminalContext,
                KeymapOverride = new TextEditorKeymapTerminal(TerminalStateWrap, TerminalKey)
            };
        }

        base.OnParametersSet();
    }
}