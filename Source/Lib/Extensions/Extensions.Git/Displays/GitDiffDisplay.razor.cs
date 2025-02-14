using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitDiffDisplay : ComponentBase
{
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    public Key<TerminalCommandRequest> GitLogTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

    private Key<TextEditorDiffModel> _textEditorDiffModelKey = Key<TextEditorDiffModel>.NewKey();
    
    public static Key<TextEditorViewModel> InViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
    public static Key<TextEditorViewModel> OutViewModelKey { get; } = Key<TextEditorViewModel>.NewKey();
    
    /// <summary>This property is bad but I have a plan in mind</summary>
    public static ResourceUri? InResourceUri { get; set; }
    
    private string? _logFileContent;
    
    private readonly Throttle _createEditorsThrottle = new Throttle(TimeSpan.FromMilliseconds(500));

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShowOriginalFromGitOnClick(
				GitBackgroundTaskApi.Git.GetGitState(),
                GitFile);
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void ShowOriginalFromGitOnClick(GitState localGitState, GitFile localGitFile)
    {
        if (localGitState.Repo is null)
            return;
            
        GitBackgroundTaskApi.Git.ShowFileEnqueue(
            localGitState.Repo,
            localGitFile.RelativePathString,
            (gitCliOutputParser, logFileContent) =>
            {
            	_createEditorsThrottle.Run(_ => { CreateEditorFromLog(gitCliOutputParser, localGitState, localGitFile, logFileContent); return Task.CompletedTask; });
            	return Task.CompletedTask;
            });
    }

	private async Task<bool> TryCreateEditorIn(string logFileContent, ResourceUri originalResourceUri)
	{
		// Dispose any previously created state for the editor in model
		if (InResourceUri is not null)
		{
			TextEditorService.ModelApi.Dispose(InResourceUri.Value);
			InResourceUri = null;
		}
		
		// Create Model
        var resourceUri = new ResourceUri(ResourceUriFacts.Diff_ReservedResourceUri_Prefix + originalResourceUri.Value);
 	   InResourceUri = resourceUri;
 	   
        var fileLastWriteTime = DateTime.UtcNow;
        var content = logFileContent;

        var absolutePath = EnvironmentProvider.AbsolutePathFactory(resourceUri.Value, false);
        var decorationMapper = DecorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
        var compilerService = CompilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

        var model = new TextEditorModel(
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
        
        model = modelModifier.ToModel();

        TextEditorService.ModelApi.RegisterCustom(model);
        
		model.CompilerService.RegisterResource(
			model.ResourceUri,
			shouldTriggerResourceWasModified: true);
			
		// Create ViewModel
		try
    	{
    		// Dispose any previously created state for the editor in view model
			TextEditorService.ViewModelApi.Dispose(InViewModelKey);
    	
    		var viewModelKey = InViewModelKey;
    		var category = new Category("diff-in");
	        
	        var viewModel = new TextEditorViewModel(
                viewModelKey,
                resourceUri,
                TextEditorService,
                PanelService,
                DialogService,
                JsRuntime,
                VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
        		new CharAndLineMeasurements(0, 0),
                false,
                category);
	
	        var firstPresentationLayerKeys = new[]
	        {
	            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
	            FindOverlayPresentationFacts.PresentationKey,
	            DiffPresentationFacts.InPresentationKey,
	        }.ToImmutableArray();
	            
	        viewModel.UnsafeState.ShouldSetFocusAfterNextRender = false;
		
            viewModel = viewModel with
            {
                GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                FirstPresentationLayerKeysList = firstPresentationLayerKeys.ToImmutableList()
            };
            
            TextEditorService.ViewModelApi.Register(viewModel);
        }
        catch (Exception e)
        {
        	NotificationHelper.DispatchError(
		        nameof(TryCreateEditorIn),
		        e.ToString(),
		        CommonComponentRenderers,
		        NotificationService,
		        TimeSpan.FromSeconds(6));
		
			return false;        
		}
		
		return true;
	}
	
	private async Task<bool> TryCreateEditorOut(ResourceUri originalResourceUri, AbsolutePath originalAbsolutePath)
	{
		// Create Model
		var originalModel = TextEditorService.ModelApi.GetOrDefault(originalResourceUri);
		if (originalModel is null)
		{
			var registerModelArgs = new RegisterModelArgs(originalResourceUri, ServiceProvider);

			await TextEditorService.TextEditorConfig.RegisterModelFunc
				.Invoke(registerModelArgs).ConfigureAwait(false);
		}
		
		// Create ViewModel
		try
    	{
    		// Dispose any previously created state for the editor out view model
			TextEditorService.ViewModelApi.Dispose(OutViewModelKey);
    	
    		var viewModelKey = OutViewModelKey;
    		var category = new Category("diff-out");
	        
	        var viewModel = new TextEditorViewModel(
                viewModelKey,
                originalResourceUri,
                TextEditorService,
                PanelService,
                DialogService,
                JsRuntime,
                VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
        		new CharAndLineMeasurements(0, 0),
                false,
                category);
	
	        var firstPresentationLayerKeys = new[]
	        {
	            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
	            FindOverlayPresentationFacts.PresentationKey,
	            DiffPresentationFacts.OutPresentationKey,
	        }.ToImmutableArray();
	            
	        viewModel.UnsafeState.ShouldSetFocusAfterNextRender = false;
		
            viewModel = viewModel with
            {
                GetTabDisplayNameFunc = _ => originalAbsolutePath.NameWithExtension,
                FirstPresentationLayerKeysList = firstPresentationLayerKeys.ToImmutableList()
            };
            
            TextEditorService.ViewModelApi.Register(viewModel);
        }
        catch (Exception e)
        {
        	NotificationHelper.DispatchError(
		        nameof(TryCreateEditorOut),
		        e.ToString(),
		        CommonComponentRenderers,
		        NotificationService,
		        TimeSpan.FromSeconds(6));
		
			return false;        
		}
		
		return true;
	}

    private async Task CreateEditorFromLog(
    	GitCliOutputParser gitCliOutputParser,
    	GitState localGitState,
    	GitFile localGitFile,
    	string logFileContent)
    {
    	/*
    	Goal: Diff Editor (2024-08-26)
    	==============================
    	Log -> string
    		-> Model (caching?)
    		   	-> new ResourceUri(ResourceUriFacts.Diff + originalResourceUri)
    		-> ViewModel (caching?)
    			   -> new Category("diff-in")
    		   	-> Add(DiffPresentationLayer)
    	Actual -> string
    		   -> Model (more likely to already exist than the log's model?)
    		   -> ViewModel (caching?)
    		   	   -> new Category("diff-out")
    		      	-> Add(DiffPresentationLayer)
    	
    	Previously I had been adding the diff presentation layer to the ViewModels of the category: "main".
    	I think this diff presentation layer can and should be removed from ViewModels of the category: "main". 
    	*/
    	
    	TextEditorService.DiffApi.Register(
            _textEditorDiffModelKey,
            InViewModelKey,
            OutViewModelKey);
    	
    	_logFileContent = logFileContent;
        if (_logFileContent is null)
            return;
        
        var absolutePathStringStandardized = await TextEditorConfig.AbsolutePathStandardizeFunc.Invoke(
        	localGitFile.AbsolutePath.Value,
        	ServiceProvider);
        	
        var originalResourceUri = new ResourceUri(absolutePathStringStandardized);
        
    	if (!(await TryCreateEditorIn(logFileContent, originalResourceUri)))
    		return;
    		
    	if (!(await TryCreateEditorOut(originalResourceUri, localGitFile.AbsolutePath)))
    		return;
    		
    	GitBackgroundTaskApi.Git.DiffFileEnqueue(
            localGitState.Repo,
            localGitFile.RelativePathString,
            (gitCliOutputParser, logFileContent, plusMarkedLineIndexList) =>
            {
            	Console.WriteLine("DiffFileEnqueue");
            	
            	TextEditorService.TextEditorWorker.PostUnique(
            		nameof(plusMarkedLineIndexList),
            		editContext =>
            		{
            			Console.WriteLine("plusMarkedLineIndexList");
            		
            			var originalResourceUri = new ResourceUri(localGitFile.AbsolutePath.Value);
            		
            			var modelModifier = editContext.GetModelModifier(originalResourceUri);
            			var viewModelModifier = editContext.GetViewModelModifier(OutViewModelKey);
            			
            			if (modelModifier is null || viewModelModifier is null)
            			{
            				Console.WriteLine("modelModifier is null || viewModelModifier is null");
            				return ValueTask.CompletedTask;
            			}
            				
            			editContext.TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
			            	editContext,
			                modelModifier,
			                DiffPresentationFacts.OutPresentationKey,
			                DiffPresentationFacts.EmptyOutPresentationModel);
			                
			            var outPresentationModel = modelModifier.PresentationModelList.First(
			                x => x.TextEditorPresentationKey == DiffPresentationFacts.OutPresentationKey);
			                
			            if (outPresentationModel.PendingCalculation is null)
			            {
			            	Console.WriteLine("outPresentationModel.PendingCalculation is null");
			                return ValueTask.CompletedTask;
			            }
			            
			            var outText = outPresentationModel.PendingCalculation.ContentAtRequest;
			            
			            var outResultTextSpanList = new List<TextEditorTextSpan>();
			            
			            foreach (var lineIndex in plusMarkedLineIndexList)
			            {
			            	
			            	var lineInformation = modelModifier.GetLineInformation(lineIndex);
			            	
			            	Console.WriteLine($"lineIndex: {lineIndex} | ({lineInformation.StartPositionIndexInclusive} to {lineInformation.UpperLineEnd.StartPositionIndexInclusive})");
			            	
			            	outResultTextSpanList.Add(new TextEditorTextSpan(
			            		lineInformation.StartPositionIndexInclusive,
			            		lineInformation.UpperLineEnd.StartPositionIndexInclusive,
							    (byte)TextEditorDiffDecorationKind.InsertionLine,
							    originalResourceUri,
							    outPresentationModel.PendingCalculation.ContentAtRequest));
			            }
			            
			            modelModifier.CompletePendingCalculatePresentationModel(
			                DiffPresentationFacts.OutPresentationKey,
			                DiffPresentationFacts.EmptyOutPresentationModel,
			                outResultTextSpanList.ToImmutableArray());
			            Console.WriteLine("modelModifier.CompletePendingCalculatePresentationModel");
            			
            			return ValueTask.CompletedTask;
            		});
            		
            	return Task.CompletedTask;
            });

        await InvokeAsync(StateHasChanged);
    }
}