using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.CodeSearches.States;

namespace Luthetus.Ide.RazorLib.CodeSearches.Displays;

public partial class CodeSearchDisplay : FluxorComponent
{
	[Inject]
	private IState<CodeSearchState> CodeSearchStateWrap { get; set; } = null!;
    [Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
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
    
    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

	private readonly ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		IncludeHeaderHelperComponent = false,
	};

    private string InputValue
	{
		get => CodeSearchStateWrap.Value.Query;
		set
		{
			if (value is null)
				value = string.Empty;

			Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
			{
				Query = value,
			}));

			Dispatcher.Dispatch(new CodeSearchState.SearchEffect());
		}
	}
	
	protected override void OnInitialized()
	{
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

		Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
		return Task.CompletedTask;
	}

	private string GetIsActiveCssClass(CodeSearchFilterKind codeSearchFilterKind)
	{
		return CodeSearchStateWrap.Value.CodeSearchFilterKind == codeSearchFilterKind
			? "luth_active"
			: string.Empty;
	}

	private async Task HandleOnClick(string filePath)
	{
		var inPreviewViewModelKey = CodeSearchStateWrap.Value.PreviewViewModelKey;
		var outPreviewViewModelKey = Key<TextEditorViewModel>.NewKey();

		var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
            return;

        await TextEditorConfig.RegisterModelFunc.Invoke(
                new RegisterModelArgs(resourceUri, ServiceProvider))
            .ConfigureAwait(false);

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    outPreviewViewModelKey,
                    resourceUri,
                    new Category(nameof(CodeSearchDisplay)),
                    false,
                    ServiceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
                TextEditorConfig.TryShowViewModelFunc is not null)
            {
                Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
                {
                    PreviewFilePath = filePath,
                    PreviewViewModelKey = viewModelKey,
                }));

                if (inPreviewViewModelKey != Key<TextEditorViewModel>.Empty &&
                    inPreviewViewModelKey != viewModelKey)
				{
                    TextEditorService.ViewModelApi.Dispose(inPreviewViewModelKey);
				}
            }
        }
    }
	
	private async Task HandleOnDoubleClick(string filePath)
	{
        var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
            return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                ServiceProvider))
            .ConfigureAwait(false);

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    resourceUri,
                    new Category("main"),
                    false,
                    ServiceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
                TextEditorConfig.TryShowViewModelFunc is not null)
            {
                await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                        viewModelKey,
                        Key<TextEditorGroup>.Empty,
                        ServiceProvider))
                    .ConfigureAwait(false);
            }
        }
    }

	private async Task HandleResizableRowReRenderAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
}