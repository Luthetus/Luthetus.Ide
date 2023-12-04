using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.TestExplorers;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewProjectTestModel : TreeViewWithType<ProjectTestModel>
{
    public TreeViewProjectTestModel(
            ProjectTestModel projectTestModel,
			ILuthetusCommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(projectTestModel, isExpandable, isExpanded)
    {
		CommonComponentRenderers = commonComponentRenderers;
    }

	public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewProjectTestModel treeViewProjectTestModel)
            return false;

        return treeViewProjectTestModel.Item.ProjectIdGuid == Item.ProjectIdGuid;
    }

    public override int GetHashCode() => Item.ProjectIdGuid.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewProjectTestModelDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewProjectTestModelDisplay.TreeViewProjectTestModel),
                    this
                },
            });
    }

    public override async Task LoadChildBagAsync()
    {
		var previousChildren = new List<TreeViewNoType>(ChildBag);
		
		ChildBag = new []
		{
			(TreeViewNoType)new TreeViewSpinner(
				Item.ProjectIdGuid,
				CommonComponentRenderers,
				false,
				false)
		}.ToList();
		
		LinkChildren(previousChildren, ChildBag);	
		
        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

		await Item.EnqueueDiscoverTestsFunc(async rootStringFragmentMap => 
		{
			try
	        {
				previousChildren = new List<TreeViewNoType>(ChildBag);

				if (rootStringFragmentMap.Values.Any())
				{
					var rootStringFragment = new StringFragment(string.Empty);
					rootStringFragment.Map = rootStringFragmentMap;
			
					var newChildBag = rootStringFragment.Map.Select(kvp => 
						(TreeViewNoType)new TreeViewStringFragment(
				            kvp.Value,
				            CommonComponentRenderers,
				            true,
				            true))
						.ToArray();
			
					for (var i = 0; i < newChildBag.Length; i++)
					{
						var node = (TreeViewStringFragment)newChildBag[i];
						await node.LoadChildBagAsync();
					}
		
		            ChildBag = newChildBag.ToList();
				}
				else
				{
					ChildBag = new List<TreeViewNoType>
		            {
		                new TreeViewException(new Exception("No results"), false, false, CommonComponentRenderers)
		                {
		                    Parent = this,
		                    IndexAmongSiblings = 0,
		                }
		            };
				}

	            LinkChildren(previousChildren, ChildBag);
	        }
	        catch (Exception exception)
	        {
	            ChildBag = new List<TreeViewNoType>
	            {
	                new TreeViewException(exception, false, false, CommonComponentRenderers)
	                {
	                    Parent = this,
	                    IndexAmongSiblings = 0,
	                }
	            };
	        }
	
	        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
			Item.ReRenderNodeAction.Invoke(this);
		});
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}