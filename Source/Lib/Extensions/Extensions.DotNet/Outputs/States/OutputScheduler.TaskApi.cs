using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeViews.Models;
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
    

        var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewNodeList);
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
