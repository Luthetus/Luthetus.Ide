using System.Collections.Immutable;
using System.Text;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Extensions.DotNet.Outputs.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial class OutputScheduler
{
	public ValueTask Task_ConstructTreeView()
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
		
		var projectManualGrouping = new Dictionary<string, TreeViewGroup>();
		var treeViewBadStateGroupList = new List<TreeViewNoType>();

		foreach (var group in filePathGrouping)
		{
			var absolutePath = _environmentProvider.AbsolutePathFactory(group.Key, false);
			var groupEnumerated = group.ToList();
			var groupNameBuilder = new StringBuilder();
			
			var errorCount = groupEnumerated.Count(x =>
				((TreeViewDiagnosticLine)x).Item.DiagnosticLineKind == DiagnosticLineKind.Error);
				
			var warningCount = groupEnumerated.Count(x =>
				((TreeViewDiagnosticLine)x).Item.DiagnosticLineKind == DiagnosticLineKind.Warning);
			
			groupNameBuilder
				.Append(absolutePath.NameWithExtension)
				.Append(" (")
				.Append(errorCount)
				.Append(" errors)")
				.Append(" (")
				.Append(warningCount)
				.Append(" warnings)");
		
			var treeViewGroup = new TreeViewGroup(
				groupNameBuilder.ToString(),
				true,
				groupEnumerated.Any(x => ((TreeViewDiagnosticLine)x).Item.DiagnosticLineKind == DiagnosticLineKind.Error))
			{
				TitleText = absolutePath.ParentDirectory?.Value ?? $"{nameof(IAbsolutePath.ParentDirectory)} was null"
			};

			treeViewGroup.ChildList = groupEnumerated;
			treeViewGroup.LinkChildren(new(), treeViewGroup.ChildList);
			
			var firstEntry = groupEnumerated.FirstOrDefault();
			
			if (firstEntry is not null)
			{
				var projectText = ((TreeViewDiagnosticLine)firstEntry).Item.ProjectTextSpan.Text;
				var projectAbsolutePath = _environmentProvider.AbsolutePathFactory(projectText, false);
			
				if (!projectManualGrouping.ContainsKey(projectText))
				{
					var treeViewGroupProject = new TreeViewGroup(
						projectAbsolutePath.NameWithExtension,
						true,
						true)
					{
						TitleText = absolutePath.ParentDirectory?.Value ?? $"{nameof(IAbsolutePath.ParentDirectory)} was null"
					};
				
					projectManualGrouping.Add(projectText, treeViewGroupProject);
				}
				
				projectManualGrouping[projectText].ChildList.Add(treeViewGroup);
			}
			else
			{
				treeViewBadStateGroupList.Add(treeViewGroup);
			}
		}
		
		var treeViewProjectGroupList = projectManualGrouping.Values
			.Select(x => (TreeViewNoType)x)
			.ToList();
			
		// Bad State
		if (treeViewBadStateGroupList.Count != 0)
		{
			var projectText = "Could not find project";
			
			var treeViewGroupProjectBadState = new TreeViewGroup(
				projectText,
				true,
				true)
			{
				TitleText = projectText
			};
			
			treeViewGroupProjectBadState.ChildList = treeViewBadStateGroupList;
		
			treeViewProjectGroupList.Add(treeViewGroupProjectBadState);
		}
		
		foreach (var treeViewProjectGroup in treeViewProjectGroupList)
		{
			treeViewProjectGroup.LinkChildren(new(), treeViewProjectGroup.ChildList);
		}
    
        var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewProjectGroupList.ToArray());
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

        _dispatcher.Dispatch(new OutputState.StateHasChangedAction(dotNetRunParseResult.Id));
        return ValueTask.CompletedTask;
    }
}
