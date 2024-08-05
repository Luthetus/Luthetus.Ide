using System.Collections.Immutable;
using System.Text;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial class OutputScheduler
{
	public Task Task_ConstructTreeView()
    {
    	var dotNetRunParseResult = _dotNetCliOutputParser.GetDotNetRunParseResult();
    	
    	var treeViewNodeList = dotNetRunParseResult.AllDiagnosticLineList.Select(x =>
    		(TreeViewNoType)new TreeViewDiagnosticLine(
    			x,
				false,
				false))
			.ToArray();
			
		var filePathGrouping = treeViewNodeList.GroupBy(
			x => ((TreeViewDiagnosticLine)x).Item.FilePathTextSpan.Text);
			
		var treeViewGroupList = new List<TreeViewNoType>();
			
		foreach (var group in filePathGrouping)
		{
			var absolutePath = _environmentProvider.AbsolutePathFactory(group.Key, false);
		
			var groupEnumerated = group.ToList();
			
			var groupNameBuilder = new StringBuilder();
			
			groupNameBuilder
				.Append(absolutePath.NameWithExtension)
				.Append("____")
				.Append(groupEnumerated.Count)
				.Append("____" + absolutePath.ParentDirectory?.Value ?? $"{nameof(IAbsolutePath.ParentDirectory)} was null");
		
			var treeViewGroup = new TreeViewGroup(
				groupNameBuilder.ToString(),
				true,
				false);
				
			treeViewGroup.ChildList = groupEnumerated;
			treeViewGroup.LinkChildren(new(), treeViewGroup.ChildList);
		
			treeViewGroupList.Add(treeViewGroup);
		}
    
        var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewGroupList.ToArray());
        var firstNode = treeViewNodeList.FirstOrDefault();

        var activeNodes = firstNode is null
            ? Array.Empty<TreeViewNoType>()
            : new[] { firstNode };

        if (!_treeViewService.TryGetTreeViewContainer(OutputState.TreeViewContainerKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                OutputState.TreeViewContainerKey,
                adhocRoot,
                activeNodes.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(OutputState.TreeViewContainerKey, adhocRoot);

            _treeViewService.SetActiveNode(
                OutputState.TreeViewContainerKey,
                firstNode,
                true,
                false);
        }

        _dispatcher.Dispatch(new OutputState.StateHasChangedAction());
        return Task.CompletedTask;
    }
}
