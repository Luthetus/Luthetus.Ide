using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.CodeSearches.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.Displays;

public partial class CodeSearchDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ICodeSearchService CodeSearchService { get; set; } = null!;
    [Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
	
	private CodeSearchTreeViewKeyboardEventHandler _treeViewKeymap = null!;
	private CodeSearchTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
	
	private Key<TextEditorViewModel> _previousTextEditorViewModelKey = Key<TextEditorViewModel>.Empty;
	private Throttle _updateContentThrottle = new Throttle(TimeSpan.FromMilliseconds(333));
    
    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));

	private readonly ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		HeaderComponentType = null,
	};

    private string InputValue
	{
		get => CodeSearchService.GetCodeSearchState().Query;
		set
		{
			if (value is null)
				value = string.Empty;

			CodeSearchService.With(inState => inState with
			{
				Query = value,
			});

			CodeSearchService.HandleSearchEffect();
		}
	}
	
	protected override void OnInitialized()
	{
		CodeSearchService.CodeSearchStateChanged += OnCodeSearchStateChanged;
		TreeViewService.TreeViewStateChanged += OnTreeViewStateChanged;
	
		_treeViewKeymap = new CodeSearchTreeViewKeyboardEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		_treeViewMouseEventHandler = new CodeSearchTreeViewMouseEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		base.OnInitialized();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		_updateContentThrottle.Run(_ => UpdateContent());
		base.OnAfterRender(firstRender);
	}
	
	private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			CodeSearchContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(CodeSearchContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(CodeSearchContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			null);

		DropdownService.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}

	private string GetIsActiveCssClass(CodeSearchFilterKind codeSearchFilterKind)
	{
		return CodeSearchService.GetCodeSearchState().CodeSearchFilterKind == codeSearchFilterKind
			? "luth_active"
			: string.Empty;
	}

	private async Task HandleResizableRowReRenderAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async Task UpdateContent()
	{
		TextEditorService.WorkerArbitrary.PostUnique(nameof(CodeSearchDisplay), async editContext =>
		{
			Console.WriteLine(nameof(UpdateContent));
		
			if (!TreeViewService.TryGetTreeViewContainer(
					CodeSearchState.TreeViewCodeSearchContainerKey,
					out var treeViewContainer))
			{
				Console.WriteLine("TryGetTreeViewContainer");
				return;
			}
			
			if (treeViewContainer.SelectedNodeList.Count > 1)
			{
				Console.WriteLine("treeViewContainer.SelectedNodeList.Count > 1");
				return;
			}
				
			var activeNode = treeViewContainer.ActiveNode;
			
			if (activeNode is not TreeViewCodeSearchTextSpan treeViewCodeSearchTextSpan)
			{
				Console.WriteLine("activeNode is not TreeViewCodeSearchTextSpan treeViewCodeSearchTextSpan");
				return;
			}
		
			var inPreviewViewModelKey = CodeSearchService.GetCodeSearchState().PreviewViewModelKey;
			var outPreviewViewModelKey = Key<TextEditorViewModel>.NewKey();
	
			var filePath = treeViewCodeSearchTextSpan.Item.ResourceUri.Value;
			var resourceUri = treeViewCodeSearchTextSpan.Item.ResourceUri;
	
	        if (TextEditorConfig.RegisterModelFunc is null)
	            return;
	
	        await TextEditorConfig.RegisterModelFunc.Invoke(
	                new RegisterModelArgs(editContext, resourceUri, ServiceProvider))
	            .ConfigureAwait(false);
	
	        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
	        {
	            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
	            		editContext,
	                    outPreviewViewModelKey,
	                    resourceUri,
	                    new Category(nameof(CodeSearchDisplay)),
	                    false,
	                    ServiceProvider))
	                .ConfigureAwait(false);
	
	            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
	                TextEditorConfig.TryShowViewModelFunc is not null)
	            {
	                CodeSearchService.With(inState => inState with
	                {
	                    PreviewFilePath = filePath,
	                    PreviewViewModelKey = viewModelKey,
	                });
	
	                if (inPreviewViewModelKey != Key<TextEditorViewModel>.Empty &&
	                    inPreviewViewModelKey != viewModelKey)
					{
						TextEditorService.ViewModelApi.Dispose(editContext, inPreviewViewModelKey);
					}
	            }
	        }
		});
    }
    
    public async void OnTreeViewStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public async void OnCodeSearchStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	CodeSearchService.CodeSearchStateChanged -= OnCodeSearchStateChanged;
    	TreeViewService.TreeViewStateChanged -= OnTreeViewStateChanged;
    }
}