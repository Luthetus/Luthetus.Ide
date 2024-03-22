using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TerminalSession> TerminalSessionKey { get; set; }

    private readonly object TerminalLock = new();

	private IntegratedTerminal _integratedTerminal = null!;
    private CancellationTokenSource _terminalCancellationTokenSource = new();
    private Task _terminalTask = Task.CompletedTask;
    private Key<TerminalSession> _seenTerminalSessionKey;

    protected override void OnInitialized()
    {
        _integratedTerminal = new CliWrapIntegratedTerminal(
            EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
            EnvironmentProvider);

        _integratedTerminal.StateChanged += IntegratedTerminal_StateChanged;

        StartTerminal();

        base.OnInitialized();
    }

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
                        new GenericDecorationMapper(),
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

    private async void IntegratedTerminal_StateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void StartTerminal()
    {
        if (_terminalTask.IsCompleted)
        {
            lock (TerminalLock)
            {
                if (_terminalTask.IsCompleted)
                {
                    _terminalCancellationTokenSource.Cancel();
                    _terminalCancellationTokenSource = new();

                    var cancellationToken = _terminalCancellationTokenSource.Token;

                    _terminalTask = Task.Run(async () => 
                    {
                        await _integratedTerminal.StartAsync(cancellationToken);
                    });

                    _terminalTask.ContinueWith(async _ =>
                    {
                        // Stop UI rendering the spinner
                        await InvokeAsync(StateHasChanged);
                    });
                }
            }
        }
    }

    private void CancelTerminalOnClick()
    {
        _terminalCancellationTokenSource.Cancel();
        _terminalCancellationTokenSource = new();
    }

	public void Dispose()
    {
        _terminalCancellationTokenSource.Cancel();
        _integratedTerminal.StateChanged -= IntegratedTerminal_StateChanged;
    }
}