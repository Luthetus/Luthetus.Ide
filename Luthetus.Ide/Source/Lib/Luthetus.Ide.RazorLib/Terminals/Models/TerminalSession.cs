using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalSession
{
    private readonly IDispatcher _dispatcher;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ITextEditorService _textEditorService;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();
    private readonly object _standardOutBuilderMapLock = new();
    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();
    private readonly Dictionary<Key<TerminalCommand>, TerminalSessionOutput> _standardOutBuilderMap = new();

    public TerminalSession(
        string displayName,
        string? workingDirectoryAbsolutePathString,
        IDispatcher dispatcher,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ICompilerServiceRegistry compilerServiceRegistry)
    {
        DisplayName = displayName;
        _dispatcher = dispatcher;
        _backgroundTaskService = backgroundTaskService;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
        _compilerServiceRegistry = compilerServiceRegistry;
        WorkingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;

        ResourceUri = new($"__LUTHETUS-{TerminalSessionKey.Guid}__");

        CreateTextEditor();
    }

	private CancellationTokenSource _commandCancellationTokenSource = new();

    private string? _previousWorkingDirectoryAbsolutePathString;
    private string? _workingDirectoryAbsolutePathString;

    public Key<TerminalSession> TerminalSessionKey { get; init; } = Key<TerminalSession>.NewKey();
    public ResourceUri ResourceUri { get; init; }
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; init; } = Key<TextEditorViewModel>.NewKey();

    public string? WorkingDirectoryAbsolutePathString
    {
        get => _workingDirectoryAbsolutePathString;
        private set
        {
            _previousWorkingDirectoryAbsolutePathString = _workingDirectoryAbsolutePathString;
            _workingDirectoryAbsolutePathString = value;

            if (_previousWorkingDirectoryAbsolutePathString != _workingDirectoryAbsolutePathString)
            {
                _textEditorService.Post(
                    nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
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

                        modelModifier.ApplyDecorationRange(terminalResource.GetTokenTextSpans());
                    });
            }
        }
    }

    public TerminalCommand? ActiveTerminalCommand { get; private set; }
	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }
    public string DisplayName { get; }

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

	/// <summary>
	/// Returns the output that occurred in the terminal session
	/// </summary>
    public string ReadStandardOut()
    {
		var output = string.Empty;

		lock(_standardOutBuilderMapLock)
		{
			var entireStdOutStringBuilder = new StringBuilder();

			foreach (var sessionOutput in _standardOutBuilderMap.Values)
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in sessionOutput.TextLineList)
				{
					perCommandStringBuilder.Append(str);
				}

				entireStdOutStringBuilder.Append(perCommandStringBuilder.ToString());
			}

			output = entireStdOutStringBuilder.ToString();
		}

        return output;
    }

	/// <summary>
	/// Returns the output that occurred as a result of a specific command
	/// </summary>
    public string? ReadStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		var output = (string?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var sessionOutput))
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in sessionOutput.TextLineList)
				{
					perCommandStringBuilder.Append(str);
				}

				output = perCommandStringBuilder.ToString();
			}
		}

        return output;
    }

	/// <summary>
	/// Returns the output that occurred in the terminal session
	/// </summary>
	public List<string>? GetStandardOut()
	{
		var allOutput = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			allOutput = _standardOutBuilderMap
				.SelectMany(kvp => GetStandardOut(kvp.Key))
				.ToList();
		}

        return allOutput;
	}

	/// <summary>
	/// Returns the output that occurred as a result of a specific command
	/// </summary>
    public List<string>? GetStandardOut(Key<TerminalCommand> terminalCommandKey)
	{
		var output = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var sessionOutput))
				output = new List<string>(sessionOutput.TextLineList); // Shallow copy to hide private memory location
		}

        return output;
	}

    public Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
		// This is the 'EnqueueCommandAsync' method, that runs terminal command
        var queueKey = BlockingBackgroundTaskWorker.GetQueueKey();

        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), queueKey,
            "Enqueue Command",
            async () =>
            {
                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    WorkingDirectoryAbsolutePathString = terminalCommand.ChangeWorkingDirectoryTo;

                if (terminalCommand.FormattedCommand.TargetFileName == "cd" &&
                    terminalCommand.FormattedCommand.ArgumentsList.Any())
                {
                    // TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
                    WorkingDirectoryAbsolutePathString = terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0);
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

                // Event stream
                {
                    var terminalCommandKey = terminalCommand.TerminalCommandKey;
					
					// Here, immediately prior to starting the terminal command,
					// a StringBuilder is being made for the respective terminal command to write to.
					//
					// I'll change this to make a new List<string>()
					lock(_standardOutBuilderMapLock)
					{
						_standardOutBuilderMap.TryAdd(terminalCommand.TerminalCommandKey, new TerminalSessionOutput());
					}

                    HasExecutingProcess = true;
                    DispatchNewStateKey();

                    try
                    {
						if (terminalCommand.BeginWith is not null)
                            await terminalCommand.BeginWith.Invoke(); // Actually start the terminal command here

                        await command.Observe(_commandCancellationTokenSource.Token)
                            .ForEachAsync(cmdEvent =>
                            {
								var output = (string?)null; // a variable for storing the output

                                switch (cmdEvent)
                                {
                                    case StartedCommandEvent started:
										lock(_standardOutBuilderMapLock)
										{
											output = $"> {terminalCommand.FormattedCommand.Value}";
	                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);

											output = $"> PID:{started.ProcessId} PWD:{WorkingDirectoryAbsolutePathString}";
	                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);

											output = null;
	                                    }

                                        break;
                                    case StandardOutputCommandEvent stdOut:
                                        output = $"{stdOut.Text}";
                                        break;
                                    case StandardErrorCommandEvent stdErr:
                                        output = $"{stdErr.Text}";
                                        break;
                                    case ExitedCommandEvent exited:
                                        output = $"Process exited; Code: {exited.ExitCode}";
                                        break;
                                }

								if (output is not null)
								{
									lock(_standardOutBuilderMapLock)
									{
                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);
                                    }
								}

                                DispatchNewStateKey();
                            });
                    }
                    catch (Exception e)
                    {
                        NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
                    }
                    finally
                    {
                        HasExecutingProcess = false;
                        DispatchNewStateKey();

                        if (terminalCommand.ContinueWith is not null)
                            await terminalCommand.ContinueWith.Invoke();
                    }
                }
            });

        return Task.CompletedTask;
    }

    public void ClearStandardOut()
    {
		lock(_standardOutBuilderMapLock)
		{
			foreach (var stringBuilder in _standardOutBuilderMap.Values)
	        {
	            stringBuilder.Clear();
	        }
		}

        DispatchNewStateKey();
    }

	public void ClearStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		lock(_standardOutBuilderMapLock)
		{
			_standardOutBuilderMap[terminalCommandKey].Clear();
		}

        DispatchNewStateKey();
    }

    public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();

        DispatchNewStateKey();
    }

    private void DispatchNewStateKey()
    {
        _dispatcher.Dispatch(new TerminalSessionState.NotifyStateChangedAction(TerminalSessionKey));
    }

    private void CreateTextEditor()
    {
        var text = "Integrated-Terminal";
        var model = new TextEditorModel(
            ResourceUri,
            DateTime.UtcNow,
            "terminal",
            $"{text}\n" +
                new string('=', text.Length) +
                "\n\n",
            new TerminalDecorationMapper(),
            _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL));

        _textEditorService.ModelApi.RegisterCustom(model);

        _textEditorService.Post(
            nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
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
            new TextEditorCategory("terminal"));

        var layerFirstPresentationKeys = new[]
        {
            TerminalPresentationFacts.PresentationKey,
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();

        _textEditorService.Post(
            nameof(TerminalSession),
            _textEditorService.ViewModelApi.WithValueFactory(
                TextEditorViewModelKey,
                textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysList = layerFirstPresentationKeys.ToImmutableList()
                    }));

        _textEditorService.Post(
            nameof(_textEditorService.ViewModelApi.MoveCursorFactory),
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

                modelModifier.ApplyDecorationRange(terminalResource.GetTokenTextSpans());
            });
    }
}