using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Outputs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Luthetus.Ide.RazorLib.Outputs.Displays;

public partial class OutputDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TerminalSessionState, TerminalSession?> TerminalSessionsStateSelection { get; set; } = null!;
    // TODO: Don't inject TerminalSessionsStateWrap. It causes too many unnecessary re-renders
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionWasModifiedState> TerminalSessionWasModifiedStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    /// <summary>
    /// <see cref="TerminalSessionKey"/> is used to narrow down the terminal
    /// session.
    /// </summary>
    [Parameter, EditorRequired]
    public Key<TerminalSession> TerminalSessionKey { get; set; } = Key<TerminalSession>.Empty;
    /// <summary>
    /// <see cref="TerminalCommandKey"/> is used to narrow down even further
    /// to the output of a specific command that was executed in a specific
    /// terminal session.
    /// <br/><br/>
    /// Optional
    /// </summary>
    [Parameter]
    public Key<TerminalCommand> TerminalCommandKey { get; set; } = Key<TerminalCommand>.Empty;
    [Parameter]
    public bool AllowInput { get; set; }
    [Parameter] // This will be 'EditorRequired'?
	// I suppose not. The default could be no parsing, just render out the text.
    public IOutputParser OutputParser { get; set; }

    private TextEditorViewModelDisplayOptions _textEditorViewModelDisplayOptions = null!;

    protected override void OnInitialized()
    {
        // Supress un-used service
        _ = TerminalSessionStateWrap;

        _textEditorViewModelDisplayOptions = new()
        {
            IncludeHeaderHelperComponent = false,
            IncludeFooterHelperComponent = false,
            IncludeContextMenuHelperComponent = false,
            AfterOnKeyDownAsyncFactory = TextEditorAfterOnKeyDownAsync,
        };

        TerminalSessionsStateSelection.Select(x =>
        {
            if (x.TerminalSessionMap.TryGetValue(TerminalSessionKey, out var terminalSession))
                return terminalSession;

            return null;
        });

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (AllowInput)
            {
                var terminalSession = TerminalSessionsStateSelection.Value;

                if (terminalSession is not null)
                {
                    var textEditorModel = TextEditorService.ModelApi.GetOrDefault(terminalSession.ResourceUri);

                    if (textEditorModel is null)
                    {
                        TextEditorService.ModelApi.RegisterTemplated(
                            ExtensionNoPeriodFacts.TXT,
                            new ResourceUri("__terminal-display-name-fallback__"),
                            DateTime.UtcNow,
                            string.Empty,
                            "TERMINAL");

                        TextEditorService.ViewModelApi.Register(
                            terminalSession.TextEditorViewModelKey,
                            terminalSession.ResourceUri,
                            new TextEditorCategory(nameof(OutputDisplay)));
                    }
                }
            }
        }

        base.OnAfterRender(firstRender);
    }

	/*
		If I want to parse the output of various streams of text.
		
		Perhaps I want to separate the parsing logic from this 'OutputDisplay' component.

		By doing so I can then pass a Blazor parameter, 'TerminalOutputParser'
		and perhaps more specifically,
		'DotNetCliOutputParser'.

		I can see in the past I've made a class named 'DotNetCliOutputLexer'
		According to my personal defintion (I'm not sure the true one)
		Lexing is a sub-step to Parsing.

		One would never 'Lex' unless they intended to 'Parse' afterwards.
		As 'Lex' is to imply converting text into objects (tokens)
		in order to allow parsing of the text to be easier.

		So, I want to rename DotNetCliOutputLexer to DotNetCliOutputParser
		because I know myself that I am only doing 1 step.

		Using { Ctrl + Shift + f } I can find all text equal to 'DotNetCliOutputLexer'

		All usages of the DotNetCliOutputLexer in this file are from comments.
	*/
    private static MarkupString ParseHttpLinks(string input)
    {
        var outputBuilder = new StringBuilder();
        var indexOfHttp = input.IndexOf("http");

        if (indexOfHttp > 0)
        {
            var firstSubstring = input[..indexOfHttp];
            var httpBuilder = new StringBuilder();
            var position = indexOfHttp;

            while (position < input.Length)
            {
                var currentCharacter = input[position++];

                if (currentCharacter == ' ') break;

                httpBuilder.Append(currentCharacter);
            }

            var aTag = $"<a href=\"{httpBuilder}\" target=\"_blank\">{httpBuilder}</a>";
            var result = firstSubstring.EscapeHtml() + aTag;

            if (position != input.Length - 1) result += input[position..];

            outputBuilder.Append(result + "<br />");
        }
        else
        {
            outputBuilder.Append(input.EscapeHtml() + "<br />");
        }

        return (MarkupString)outputBuilder.ToString();
    }

    private TextEditorEdit TextEditorAfterOnKeyDownAsync(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModel,
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task> setTextEditorMenuKind)
    {
        return editContext =>
        {
            // TODO: I am commenting this code out as of (2023-12-07). I am currently rewriting the...
            // ...text editor to be immutable. I don't know why this method is here and it isn't used?
            //
            //if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
            //{
            //    var text = textEditor.GetAllText();
            //    textEditor.WithContent(string.Empty);

            //    var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[
            //        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            //    var whitespace = new[]
            //    {
            //        KeyboardKeyFacts.WhitespaceCharacters.SPACE,
            //        KeyboardKeyFacts.WhitespaceCharacters.TAB,
            //        KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE,
            //        KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN,
            //    };

            //    var indexOfFirstWordEndingExclusive = text.IndexOfAny(whitespace);

            //    var targetFileName = text[..indexOfFirstWordEndingExclusive];

            //    if (targetFileName.StartsWith('.'))
            //    {
            //        targetFileName = (generalTerminalSession.WorkingDirectoryAbsolutePathString ?? string.Empty) +
            //            targetFileName;
            //    }

            //    var arguments = text[(indexOfFirstWordEndingExclusive + 1)..]
            //        .Split(whitespace)
            //        .Where(x => !string.IsNullOrWhiteSpace(x))
            //        .ToArray();

            //    var formattedCommand = new FormattedCommand(targetFileName, arguments);

            //    var terminalCommand = new TerminalCommand(
            //        Key<TerminalCommand>.NewKey(),
            //        formattedCommand,
            //        null,
            //        CancellationToken.None,
            //        () => Task.CompletedTask);

            //    await generalTerminalSession.EnqueueCommandAsync(terminalCommand).ConfigureAwait(false);
            //}

            return Task.CompletedTask;
        };
    }
}