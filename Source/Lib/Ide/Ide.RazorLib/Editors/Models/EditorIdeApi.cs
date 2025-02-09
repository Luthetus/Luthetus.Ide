using System.Collections.Immutable;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Editors.Models;

public class EditorIdeApi
{
    public static readonly Key<TextEditorGroup> EditorTextEditorGroupKey = Key<TextEditorGroup>.NewKey();
    
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ITextEditorService _textEditorService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDecorationMapperRegistry _decorationMapperRegistry;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly IDialogService _dialogService;
    private readonly IDispatcher _dispatcher;
    private readonly IJSRuntime _jsRuntime;
    private readonly IServiceProvider _serviceProvider;

    public EditorIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers,
        IIdeComponentRenderers ideComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDecorationMapperRegistry decorationMapperRegistry,
        ICompilerServiceRegistry compilerServiceRegistry,
        IDialogService dialogService,
        IDispatcher dispatcher,
        IJSRuntime jsRuntime,
        IServiceProvider serviceProvider)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _backgroundTaskService = backgroundTaskService;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
        _ideComponentRenderers = ideComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _decorationMapperRegistry = decorationMapperRegistry;
        _compilerServiceRegistry = compilerServiceRegistry;
        _dialogService = dialogService;
        _dispatcher = dispatcher;
        _jsRuntime = jsRuntime;
        _serviceProvider = serviceProvider;
    }

    public void ShowInputFile()
    {
        _ideBackgroundTaskApi.InputFile.RequestInputFileStateForm(
            "TextEditor",
            absolutePath =>
            {
                return _textEditorService.OpenInEditorAsync(
					absolutePath.Value,
					true,
					null,
					new Category("main"),
					Key<TextEditorViewModel>.NewKey());
            },
            absolutePath =>
            {
                if (absolutePath.ExactInput is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                    new InputFilePattern("File", absolutePath => !absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    public async Task RegisterModelFunc(RegisterModelArgs registerModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerModelArgs.ResourceUri);

        if (model is null)
        {
            var resourceUri = registerModelArgs.ResourceUri;

            var fileLastWriteTime = await _fileSystemProvider.File
                .GetLastWriteTimeAsync(resourceUri.Value)
                .ConfigureAwait(false);

            var content = await _fileSystemProvider.File
                .ReadAllTextAsync(resourceUri.Value)
                .ConfigureAwait(false);

            var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUri.Value, false);
            var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            model = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);
                
            var modelModifier = new TextEditorModelModifier(model);
            modelModifier.PerformRegisterPresentationModelAction(CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);
            modelModifier.PerformRegisterPresentationModelAction(FindOverlayPresentationFacts.EmptyPresentationModel);
            modelModifier.PerformRegisterPresentationModelAction(DiffPresentationFacts.EmptyInPresentationModel);
            modelModifier.PerformRegisterPresentationModelAction(DiffPresentationFacts.EmptyOutPresentationModel);
            
            model = modelModifier.ToModel();

            _textEditorService.ModelApi.RegisterCustom(model);
            
			model.CompilerService.RegisterResource(
				model.ResourceUri,
				shouldTriggerResourceWasModified: false);
				
			var uniqueTextEditorWork = new UniqueTextEditorWork(
	            nameof(compilerService.ParseAsync),
	            _textEditorService,
	            editContext =>
	            {
					var modelModifier = editContext.GetModelModifier(resourceUri);
	
					if (modelModifier is null)
						return ValueTask.CompletedTask;
	
					return compilerService.ParseAsync(editContext, modelModifier, shouldApplySyntaxHighlighting: false);
	            });
			
			if (registerModelArgs.ShouldBlockUntilBackgroundTaskIsCompleted)
				await _textEditorService.PostAsync(uniqueTextEditorWork).ConfigureAwait(false);
			else
				_textEditorService.TextEditorWorker.EnqueueUniqueTextEditorWork(uniqueTextEditorWork);
        }

        await CheckIfContentsWereModifiedAsync(
                _dispatcher,
                registerModelArgs.ResourceUri.Value,
                model)
            .ConfigureAwait(false);
    }

    public Task<Key<TextEditorViewModel>> TryRegisterViewModelFunc(TryRegisterViewModelArgs registerViewModelArgs)
    {
    	try
    	{
	        var model = _textEditorService.ModelApi.GetOrDefault(registerViewModelArgs.ResourceUri);
	
	        if (model is null)
	        {
	        	NotificationHelper.DispatchDebugMessage(nameof(TryRegisterViewModelFunc), () => "model is null: " + registerViewModelArgs.ResourceUri.Value, _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(4));
	            return Task.FromResult(Key<TextEditorViewModel>.Empty);
	        }
	
	        var viewModel = _textEditorService.ModelApi
	            .GetViewModelsOrEmpty(registerViewModelArgs.ResourceUri)
	            .FirstOrDefault(x => x.Category == registerViewModelArgs.Category);
	
	        if (viewModel is not null)
			    return Task.FromResult(viewModel.ViewModelKey);
	
	        var viewModelKey = Key<TextEditorViewModel>.NewKey();
	
	        viewModel = new TextEditorViewModel(
                viewModelKey,
                registerViewModelArgs.ResourceUri,
                _textEditorService,
                _dispatcher,
                _dialogService,
                _jsRuntime,
                VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
        		new CharAndLineMeasurements(0, 0),
                false,
                registerViewModelArgs.Category);
	
	        var firstPresentationLayerKeys = new[]
	        {
	            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
	            FindOverlayPresentationFacts.PresentationKey,
	        }.ToImmutableArray();
	
	        var absolutePath = _environmentProvider.AbsolutePathFactory(
	            registerViewModelArgs.ResourceUri.Value,
	            false);
	            
	        viewModel.UnsafeState.ShouldSetFocusAfterNextRender = registerViewModelArgs.ShouldSetFocusToEditor;
		
            viewModel = viewModel with
            {
                OnSaveRequested = HandleOnSaveRequested,
                GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                FirstPresentationLayerKeysList = firstPresentationLayerKeys.ToImmutableList()
            };
            
            _textEditorService.ViewModelApi.Register(viewModel);
	
	        return Task.FromResult(viewModelKey);

	        void HandleOnSaveRequested(ITextEditorModel innerTextEditor)
	        {
	            var innerContent = innerTextEditor.GetAllText();
	
	            var cancellationToken = model.TextEditorSaveFileHelper.GetCancellationToken();
	
	            _ideBackgroundTaskApi.FileSystem.SaveFile(
	                absolutePath,
	                innerContent,
	                writtenDateTime =>
	                {
	                    if (writtenDateTime is not null)
	                    {
	                        _textEditorService.TextEditorWorker.PostUnique(
	                            nameof(HandleOnSaveRequested),
	                            editContext =>
	                            {
	                            	var modelModifier = editContext.GetModelModifier(innerTextEditor.ResourceUri);
	                            	if (modelModifier is null)
	                            		return ValueTask.CompletedTask;
	                            
	                            	_textEditorService.ModelApi.SetResourceData(
	                            		editContext,
		                                modelModifier,
		                                writtenDateTime.Value);
	                                return ValueTask.CompletedTask;
	                            });
	                    }
	
	                    return Task.CompletedTask;
	                },
	                cancellationToken);
	        }
        }
        catch (Exception e)
        {
        	NotificationHelper.DispatchError(
		        nameof(TryRegisterViewModelFunc),
		        e.ToString(),
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(6));
		    return Task.FromResult(Key<TextEditorViewModel>.Empty);
        }
    }

    public async Task<bool> TryShowViewModelFunc(TryShowViewModelArgs showViewModelArgs)
    {
        _textEditorService.GroupApi.Register(EditorTextEditorGroupKey);

        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(showViewModelArgs.ViewModelKey);

        if (viewModel is null)
            return false;

        if (viewModel.Category == new Category("main") &&
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            showViewModelArgs = new TryShowViewModelArgs(
                showViewModelArgs.ViewModelKey,
                EditorTextEditorGroupKey,
                showViewModelArgs.ShouldSetFocusToEditor,
                showViewModelArgs.ServiceProvider);
        }

        if (showViewModelArgs.ViewModelKey == Key<TextEditorViewModel>.Empty ||
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            return false;
        }

        _textEditorService.GroupApi.AddViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        _textEditorService.GroupApi.SetActiveViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);
            
        if (showViewModelArgs.ShouldSetFocusToEditor)
        {
        	_textEditorService.TextEditorWorker.PostUnique(nameof(TryShowViewModelFunc), editContext =>
	        {
	        	var viewModelModifier = editContext.GetViewModelModifier(showViewModelArgs.ViewModelKey);
	        	
	        	viewModelModifier.ViewModel.UnsafeState.ShouldSetFocusAfterNextRender =
	        		showViewModelArgs.ShouldSetFocusToEditor;
	        		
	        	return viewModel.FocusAsync();
	        });
        }

        return true;
    }

    private async Task CheckIfContentsWereModifiedAsync(
        IDispatcher dispatcher,
        string inputFileAbsolutePathString,
        TextEditorModel textEditorModel)
    {
        var fileLastWriteTime = await _fileSystemProvider.File
            .GetLastWriteTimeAsync(inputFileAbsolutePathString)
            .ConfigureAwait(false);

        if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
            _ideComponentRenderers.BooleanPromptOrCancelRendererType is not null)
        {
            var notificationInformativeKey = Key<IDynamicViewModel>.NewKey();

            var notificationInformative = new NotificationViewModel(
                notificationInformativeKey,
                "File contents were modified on disk",
                _ideComponentRenderers.BooleanPromptOrCancelRendererType,
                new Dictionary<string, object?>
                {
                        {
                            nameof(IBooleanPromptOrCancelRendererType.Message),
                            "File contents were modified on disk"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.AcceptOptionTextOverride),
                            "Reload"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptFunc),
                            new Func<Task>(() =>
                            {
                                _backgroundTaskService.Enqueue(
                                        Key<IBackgroundTask>.NewKey(),
                                        BackgroundTaskFacts.ContinuousQueueKey,
                                        "Check If Contexts Were Modified",
                                        async () =>
                                        {
                                            dispatcher.Dispatch(new NotificationState.DisposeAction(
                                                notificationInformativeKey));

                                            var content = await _fileSystemProvider.File
                                                .ReadAllTextAsync(inputFileAbsolutePathString)
                                                .ConfigureAwait(false);

                                            _textEditorService.TextEditorWorker.PostUnique(
                                                nameof(CheckIfContentsWereModifiedAsync),
                                                editContext =>
                                                {
                                                	var modelModifier = editContext.GetModelModifier(textEditorModel.ResourceUri);
                                                	if (modelModifier is null)
                                                		return ValueTask.CompletedTask;
                                                
                                                    _textEditorService.ModelApi.Reload(
                                                    	editContext,
                                                        modelModifier,
                                                        content,
                                                        fileLastWriteTime);

                                                    editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                                                    	editContext,
                                                        modelModifier);
                                                	return ValueTask.CompletedTask;
                                                });
                                        });
								return Task.CompletedTask;
							})
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineFunc),
                            new Func<Task>(() =>
                            {
                                dispatcher.Dispatch(new NotificationState.DisposeAction(
                                    notificationInformativeKey));

                                return Task.CompletedTask;
                            })
                        },
                },
                TimeSpan.FromSeconds(20),
                true,
                null);

            dispatcher.Dispatch(new NotificationState.RegisterAction(
                notificationInformative));
        }
    }
}
