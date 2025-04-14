/*
// FindAllReferences
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Ide.RazorLib.FindAllReferences.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Displays;

public partial class FindAllReferencesDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IFindAllReferencesService FindAllReferencesService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;

	public static readonly Key<Panel> FindAllReferencesPanelKey = Key<Panel>.NewKey();
    public static readonly Key<IDynamicViewModel> FindAllReferencesDynamicViewModelKey = Key<IDynamicViewModel>.NewKey();
    
    private IExtendedCompilerService _cSharpCompilerService = null!;
    
    private FindAllReferencesTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
	private FindAllReferencesTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
	*/

	// FindAllReferences
	// private int OffsetPerDepthInPixels => (int)Math.Ceiling(
	// 	AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));
    
    /*
    // FindAllReferences
    protected override void OnInitialized()
    {
    	FindAllReferencesService.FindAllReferencesStateChanged += OnFindAllReferencesStateChanged;
    	
    	_cSharpCompilerService = (IExtendedCompilerService)CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);
    	
    	_treeViewKeyboardEventHandler = new FindAllReferencesTreeViewKeyboardEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		_treeViewMouseEventHandler = new FindAllReferencesTreeViewMouseEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);
    	
    	base.OnInitialized();
    }
    
    private async void OnFindAllReferencesStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			FindAllReferencesContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(FindAllReferencesContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(FindAllReferencesContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			restoreFocusOnClose: null);

		DropdownService.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}
    
    public void Dispose()
    {
    	FindAllReferencesService.FindAllReferencesStateChanged -= OnFindAllReferencesStateChanged;
    }
}
*/
