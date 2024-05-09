using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.States;
using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Ide.RazorLib.Events.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class Terminal
{
    private readonly IDispatcher _dispatcher;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ITextEditorService _textEditorService;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();
    private readonly Dictionary<Key<TerminalCommand>, TextEditorTextSpan> _terminalCommandTextSpanMap = new();
    private readonly Dictionary<Key<TerminalCommand>, Key<TextEditorViewModel>> _terminalCommandViewModelKeyMap = new();

    public Terminal(
        string displayName,
        string? workingDirectoryAbsolutePathString,
        IDispatcher dispatcher,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ICompilerServiceRegistry compilerServiceRegistry)
    {
        _dispatcher = dispatcher;
        _backgroundTaskService = backgroundTaskService;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
        _compilerServiceRegistry = compilerServiceRegistry;

        DisplayName = displayName;
        WorkingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;
        ResourceUri = new(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Key.Guid.ToString());

        CreateTextEditor();
    }

	private CancellationTokenSource _commandCancellationTokenSource = new();
    private string? _previousWorkingDirectoryAbsolutePathString;
    private string? _workingDirectoryAbsolutePathString;

    public Key<Terminal> Key { get; init; } = Key<Terminal>.NewKey();
    public ResourceUri ResourceUri { get; init; }
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; init; } = Key<TextEditorViewModel>.NewKey();
    public TerminalCommand? ActiveTerminalCommand { get; private set; }
	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }
    public string DisplayName { get; }

    public string? WorkingDirectoryAbsolutePathString
    {
        get => _workingDirectoryAbsolutePathString;
        private set
        {
            _previousWorkingDirectoryAbsolutePathString = _workingDirectoryAbsolutePathString;
            _workingDirectoryAbsolutePathString = value;

            if (_previousWorkingDirectoryAbsolutePathString != _workingDirectoryAbsolutePathString)
            {
                WriteWorkingDirectory();
            }
        }
    }

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

    public Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
        var queueKey = BlockingBackgroundTaskWorker.GetQueueKey();

        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), queueKey,
            "Enqueue Command",
            async () =>
            {
                MoveCursorToEnd();

                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    WorkingDirectoryAbsolutePathString = terminalCommand.ChangeWorkingDirectoryTo;

                if (terminalCommand.FormattedCommand.TargetFileName == "cd")
                {
                    // TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
                    if (terminalCommand.FormattedCommand.HACK_ArgumentsString is not null)
                        WorkingDirectoryAbsolutePathString = terminalCommand.FormattedCommand.HACK_ArgumentsString;
                    else if (terminalCommand.FormattedCommand.ArgumentsList.Any())
                        WorkingDirectoryAbsolutePathString = terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0);

                    return;
                }
                
                if (terminalCommand.FormattedCommand.TargetFileName == "clear")
                {
                    ClearTerminal();
                    WriteWorkingDirectory();
                    return;
                }

                _terminalCommandsHistory.Add(terminalCommand);
                ActiveTerminalCommand = terminalCommand;

                var command = Cli.Wrap(terminalCommand.FormattedCommand.TargetFileName);

                if (terminalCommand.FormattedCommand.ArgumentsList.Any())
                {
                    if (terminalCommand.FormattedCommand.HACK_ArgumentsString is null)
                        command = command.WithArguments(terminalCommand.FormattedCommand.ArgumentsList);
                    else
                        command = command.WithArguments(terminalCommand.FormattedCommand.HACK_ArgumentsString);
                }

                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    command = command.WithWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);
                else if (WorkingDirectoryAbsolutePathString is not null)
                    command = command.WithWorkingDirectory(WorkingDirectoryAbsolutePathString);

                try
                {
                    var terminalCommandKey = terminalCommand.TerminalCommandKey;
                    HasExecutingProcess = true;
                    DispatchNewStateKey();

                    if (_textEditorService.ModelApi.GetOrDefault(new ResourceUri("terminalCommand" + '_' + terminalCommandKey)) is not null)
                        ClearOutputView(terminalCommand);

                    if (terminalCommand.BeginWith is not null)
                        await terminalCommand.BeginWith.Invoke();

                    var terminalCommandBoundary = new TerminalCommandBoundary();

                    var outputOffset = 0;

                    await command.Observe(_commandCancellationTokenSource.Token)
                        .ForEachAsync(cmdEvent =>
                        {
							var output = (string?)null;

                            switch (cmdEvent)
                            {
                                case StartedCommandEvent started:
                                    // TODO: If the source of the terminal command is a user having...
                                    //       ...typed themselves, then hitting enter, do not write this out.
                                    //       |
                                    //       This is here for when the command was started programmatically
                                    //       without a user typing into the terminal.
                                    output = $"{terminalCommand.FormattedCommand.Value}\n";
									break;
                                case StandardOutputCommandEvent stdOut:
                                    output = $"{stdOut.Text}\n";
                                    break;
                                case StandardErrorCommandEvent stdErr:
                                    output = $"{stdErr.Text}\n";
                                    break;
                                case ExitedCommandEvent exited:
                                    output = $"Process exited; Code: {exited.ExitCode}\n";
                                    break;
                            }

							if (output is not null)
                            {
                                var outputTextSpanList = new List<TextEditorTextSpan>();

                                if (terminalCommand.OutputParser is not null)
                                    outputTextSpanList = terminalCommand.OutputParser.ParseLine(output);

                                _textEditorService.Post(new OnOutput(
                                    outputOffset,
                                    output,
                                    outputTextSpanList,
                                    ResourceUri,
                                    _textEditorService,
                                    terminalCommandBoundary,
                                    TextEditorViewModelKey));

                                outputOffset += output.Length;
                            }

                            DispatchNewStateKey();
                        });

                    if (!_terminalCommandTextSpanMap.ContainsKey(terminalCommandKey))
                    {
                        _terminalCommandTextSpanMap.Add(
                            terminalCommandKey,
                            new TextEditorTextSpan(
                                terminalCommandBoundary.StartPositionIndexInclusive ?? 0,
                                terminalCommandBoundary.EndPositionIndexExclusive ?? 0,
                                0,
                                ResourceUri,
                                _textEditorService.ModelApi.GetAllText(ResourceUri) ?? string.Empty));
                    }
                    else
                    {
                        _textEditorService.PostSimpleBatch(
                            "set-content_" + terminalCommandKey.Guid,
                            string.Empty,
                            editContext =>
                            {
                                var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommandKey);
                                var modelModifier = editContext.GetModelModifier(commandOutputResourceUri);

                                if (modelModifier is null)
                                    return Task.CompletedTask;

                                var textSpan = new TextEditorTextSpan(
                                    terminalCommandBoundary.StartPositionIndexInclusive ?? 0,
                                    terminalCommandBoundary.EndPositionIndexExclusive ?? 0,
                                    0,
                                    ResourceUri,
                                    _textEditorService.ModelApi.GetAllText(ResourceUri) ?? string.Empty);

                                _terminalCommandTextSpanMap[terminalCommandKey] = textSpan;

                                modelModifier.SetContent(textSpan.GetText());
                                return Task.CompletedTask;
                            });
                    }
                }
                catch (Exception e)
                {
                    NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
                }
                finally
                {
                    HasExecutingProcess = false;
                    WriteWorkingDirectory();
                    DispatchNewStateKey();

                    if (terminalCommand.ContinueWith is not null)
                        await terminalCommand.ContinueWith.Invoke();

                    terminalCommand.OutputParser?.Dispose();
                }
            });
    }

    /// <summary>
    /// When using the <see cref="Terminal"/>, one can run many <see cref="TerminalCommand"/>.
    /// Therefore, a way to filter the <see cref="Terminal"/>, such that only the output of a single
    /// <see cref="TerminalCommand"/> is returned, is needed.
    /// </summary>
    public bool TryGetTerminalCommandTextSpan(Key<TerminalCommand> terminalCommandKey, out TextEditorTextSpan? textSpan)
    {
        return _terminalCommandTextSpanMap.TryGetValue(terminalCommandKey, out textSpan);
    }

	public bool TryGetTerminalCommandViewModelKey(
        Key<TerminalCommand> terminalCommandKey,
        out Key<TextEditorViewModel> viewModelKey)
	{
		return _terminalCommandViewModelKeyMap.TryGetValue(terminalCommandKey, out viewModelKey);
	}

	public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();
        DispatchNewStateKey();
    }

	public void CreateTextEditorForCommandOutput(Key<TerminalCommand> terminalCommandKey, Key<TextEditorViewModel> commandOutputViewModelKey)
	{
        var success = TryGetTerminalCommandTextSpan(terminalCommandKey, out var textSpan);

        if (!success || textSpan is null)
            return;

        var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommandKey);

		var model = new TextEditorModel(
			commandOutputResourceUri,
            DateTime.UtcNow,
            "terminal",
            textSpan.GetText(),
            null, //new TerminalDecorationMapper(),
            null); //_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

		_textEditorService.ModelApi.RegisterCustom(model);

		_textEditorService.PostSimpleBatch(
			nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
            string.Empty,
            async editContext =>
			{
				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						TerminalPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext);

				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext);

				await _textEditorService.ModelApi.AddPresentationModelFactory(
						model.ResourceUri,
						FindOverlayPresentationFacts.EmptyPresentationModel)
					.Invoke(editContext);

				model.CompilerService.RegisterResource(model.ResourceUri);

				_terminalCommandViewModelKeyMap.Add(terminalCommandKey, commandOutputViewModelKey);

				_textEditorService.ViewModelApi.Register(
					commandOutputViewModelKey,
					commandOutputResourceUri,
					new Category("terminal"));

                var viewModelModifier = editContext.GetViewModelModifier(commandOutputViewModelKey);

                if (viewModelModifier is null)
                    throw new NullReferenceException();

				var layerFirstPresentationKeys = new[]
				{
			        TerminalPresentationFacts.PresentationKey,
			        CompilerServiceDiagnosticPresentationFacts.PresentationKey,
			        FindOverlayPresentationFacts.PresentationKey,
		        }.ToImmutableArray();

                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
					FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
				};
			});
	}

	private void DispatchNewStateKey()
    {
        _dispatcher.Dispatch(new TerminalState.NotifyStateChangedAction(Key));
    }

    private void CreateTextEditor()
    {
        var line1 = "Integrated-Terminal";
        var line2 = "Try: cmd /c \"dir\"";

        var longestLineLength = Math.Max(line1.Length, line2.Length);

        var model = new TextEditorModel(
            ResourceUri,
            DateTime.UtcNow,
            "terminal",
            $"{line1}\n" +
                $"{line2}\n" +
                new string('=', longestLineLength) +
                "\n\n",
            new TerminalDecorationMapper(),
            _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

        _textEditorService.ModelApi.RegisterCustom(model);

        _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
            string.Empty,
            async editContext =>
            {
                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        TerminalPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext);

                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext);

                await _textEditorService.ModelApi.AddPresentationModelFactory(
                        model.ResourceUri,
                        FindOverlayPresentationFacts.EmptyPresentationModel)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                model.CompilerService.RegisterResource(model.ResourceUri);
            });

        _textEditorService.ViewModelApi.Register(
            TextEditorViewModelKey,
            ResourceUri,
            new Category("terminal"));

        var layerFirstPresentationKeys = new[]
        {
            TerminalPresentationFacts.PresentationKey,
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();

        _textEditorService.PostSimpleBatch(
            nameof(Terminal),
            string.Empty,
            _textEditorService.ViewModelApi.WithValueFactory(
                TextEditorViewModelKey,
                textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                    }));

        _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await _textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Code = KeyboardKeyFacts.MovementKeys.END,
                            Key = KeyboardKeyFacts.MovementKeys.END,
                            CtrlKey = true,
                        },
                        ResourceUri,
                        TextEditorViewModelKey)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    0,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Comment,
                    ResourceUri,
                    modelModifier.GetAllText()));

                await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                        modelModifier.ResourceUri,
                        terminalResource.GetTokenTextSpans())
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }

    public void WriteWorkingDirectory()
    {
        _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var startingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

                await _textEditorService.ModelApi.InsertTextFactory(
                        ResourceUri,
                        TextEditorViewModelKey,
                        (WorkingDirectoryAbsolutePathString ?? "null") + '>',
                        CancellationToken.None)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Add(new TextEditorTextSpan(
                    startingPositionIndex,
                    modelModifier.GetPositionIndex(primaryCursorModifier),
                    (byte)TerminalDecorationKind.Keyword,
                    ResourceUri,
                    modelModifier.GetAllText()));

                await editContext.TextEditorService.ModelApi.ApplyDecorationRangeFactory(
                        modelModifier.ResourceUri,
                        terminalResource.GetTokenTextSpans())
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }
    
    public void MoveCursorToEnd()
    {
        _textEditorService.PostSimpleBatch(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await _textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Code = KeyboardKeyFacts.MovementKeys.END,
                            Key = KeyboardKeyFacts.MovementKeys.END,
                            CtrlKey = true,
                        },
                        ResourceUri,
                        TextEditorViewModelKey)
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            });
    }

    public void ClearTerminal()
    {
        _textEditorService.PostSimpleBatch(
            nameof(ClearTerminal),
            string.Empty,
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(TextEditorViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                await TextEditorCommandDefaultFunctions.SelectAllFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        null)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                await _textEditorService.ModelApi.DeleteTextByMotionFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        MotionKind.Delete,
                        CancellationToken.None)
                    .Invoke(editContext)
                    .ConfigureAwait(false);

                var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;
                if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                    return;

                terminalResource.ManualDecorationTextSpanList.Clear();
                terminalResource.SyntaxTokenList.Clear();
            });
    }

    /// <summary>
    /// This method refers to a substring of the terminal output.
    /// For example, the unit test explorer, will show terminal output,
    /// which is the result of taking a substring from the entire terminal output.
    /// </summary>
    private void ClearOutputView(TerminalCommand terminalCommand)
    {
        _textEditorService.PostSimpleBatch(
            "clear-content_" + terminalCommand.TerminalCommandKey.Guid,
            string.Empty,
            editContext =>
            {
                var commandOutputResourceUri = new ResourceUri("terminalCommand" + '_' + terminalCommand.TerminalCommandKey);
                var modelModifier = editContext.GetModelModifier(commandOutputResourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                var textSpan = TextEditorTextSpan.FabricateTextSpan($"> {terminalCommand.FormattedCommand.Value}\n");

                _terminalCommandTextSpanMap[terminalCommand.TerminalCommandKey] = textSpan;

                modelModifier.SetContent(textSpan.GetText());
                return Task.CompletedTask;
            });
    }
}