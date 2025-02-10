using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

/// <summary>
/// The purpose of this component is to 'dump' all of the data which resides
/// in a <see cref="Luthetus.Common.RazorLib.TreeViews.Models.TreeViewContainer"/>
/// to the UI.
/// <br/>
/// The plan is to render this component
/// in a <see cref="Luthetus.Common.RazorLib.Dialogs.Displays.DialogDisplay"/>.
/// </summary>
public partial class TreeViewDebugInfo : ComponentBase, IDisposable
{
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;

	[Parameter, EditorRequired]
	public Key<TreeViewContainer> TreeViewContainerKey { get; set; } = Key<TreeViewContainer>.Empty;
	[Parameter]
	public Func<List<TreeViewNoType>, TreeViewNoType, Task>? RecursiveGetFlattenedTreeFunc { get; set; }

	private List<TreeViewNoType> _nodeList = new();

	protected override void OnInitialized()
    {
        TreeViewService.TreeViewStateChanged += OnTreeViewStateChanged;

        base.OnInitialized();
    }

	private async Task PerformGetFlattenedTree()
	{
		if (RecursiveGetFlattenedTreeFunc is null)
			return;

		_nodeList.Clear();

		await RecursiveGetFlattenedTreeFunc
			.Invoke(_nodeList, TreeViewService.GetTreeViewContainer(TreeViewContainerKey).RootNode)
            .ConfigureAwait(false);
	}
	
	public async void OnTreeViewStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		TreeViewService.TreeViewStateChanged -= OnTreeViewStateChanged;
	}
}