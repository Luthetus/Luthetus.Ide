using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Terminals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
                    // This UI element acts as a 'pseudo terminal'. 
                    // The writer of this comment believes terminals work in the following way
                    // NOTE: (may or may not be correct on this)
                    //
                    // Pseudo-terminal -> terminal (tty) -> shell -> kernel
                    //
                    // In the case of this UI element we can replace the previous diagram with
                    //
                    // TerminalDisplay.razor -> CliWrap (C# Library) -> bash or (something else) -> kernel
                    var text = "Luthetus.Ide::Integrated-Terminal";
                    var model = new TextEditorModel(
                        terminalSession.ResourceUri,
                        DateTime.UtcNow,
                        "terminal",
                        $"{text}\n" +
                            new string('=', text.Length) +
                            "\n\n",
                        new TerminalDecorationMapper(),
                        new TerminalCompilerService(TextEditorService));

                    TextEditorService.ModelApi.RegisterCustom(model);

                    TextEditorService.ViewModelApi.Register(
                        terminalSession.TextEditorViewModelKey,
                        terminalSession.ResourceUri,
                        new TextEditorCategory("terminal"));

                    TextEditorService.Post(nameof(TextEditorService.ViewModelApi.MoveCursorFactory), TextEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Code = KeyboardKeyFacts.MovementKeys.END,
                            Key = KeyboardKeyFacts.MovementKeys.END,
                            CtrlKey = true,
                        },
                        terminalSession.ResourceUri,
                        terminalSession.TextEditorViewModelKey));
                }
            }
        }

        base.OnParametersSet();
    }
}