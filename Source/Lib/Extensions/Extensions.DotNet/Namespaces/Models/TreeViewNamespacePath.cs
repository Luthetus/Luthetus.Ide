using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.Namespaces.Models;

public class TreeViewNamespacePath : TreeViewWithType<NamespacePath>
{
    public TreeViewNamespacePath(
            NamespacePath namespacePath,
            IDotNetComponentRenderers dotNetComponentRenderers,
            IIdeComponentRenderers ideComponentRenderers,
            ICommonComponentRenderers commonComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(namespacePath, isExpandable, isExpanded)
    {
        DotNetComponentRenderers = dotNetComponentRenderers;
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public IDotNetComponentRenderers DotNetComponentRenderers { get; }
    public IIdeComponentRenderers IdeComponentRenderers { get; }
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewNamespacePath treeViewSolutionExplorer)
            return false;

        return treeViewSolutionExplorer.Item.AbsolutePath.Value ==
               Item.AbsolutePath.Value;
    }

    public override int GetHashCode() => Item.AbsolutePath.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.IdeTreeViews.TreeViewNamespacePathRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewNamespacePathRendererType.NamespacePath),
                    Item
                },
            });
    }

    public override async Task LoadChildListAsync()
    {
    	// Codebehinds: Children need to
    	// - new instance
		// - try map to old instance
		// - opportunity for child to do something (like take siblings as their children)
    
        try
        {
            var previousChildren = new List<TreeViewNoType>(ChildList);
            var newChildList = new List<TreeViewNoType>();

			// new instance
            if (Item.AbsolutePath.IsDirectory)
            {
                newChildList = await TreeViewHelperNamespacePathDirectory.LoadChildrenAsync(this).ConfigureAwait(false);
            }
            else
            {
                switch (Item.AbsolutePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                        return;
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        newChildList = await TreeViewHelperCSharpProject.LoadChildrenAsync(this).ConfigureAwait(false);
                        break;
                    case ExtensionNoPeriodFacts.RAZOR_MARKUP:
                        newChildList = await TreeViewHelperRazorMarkup.LoadChildrenAsync(this).ConfigureAwait(false);
                        break;
                }
            }

			// try map to old instance
            ChildList = newChildList;
            LinkChildren(previousChildren, ChildList);
            
            // opportunity for child to do something (like take siblings as their children)
            {
	            var shouldPermitChildToTakeSiblingsAsChildren = false;
	            
	            if (Item.AbsolutePath.IsDirectory)
	            {
	                shouldPermitChildToTakeSiblingsAsChildren = true;
	            }
	            else
	            {
	                switch (Item.AbsolutePath.ExtensionNoPeriod)
	                {
	                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
	                        shouldPermitChildToTakeSiblingsAsChildren = true;
	                        break;
	                }
	            }
	            
	            if (shouldPermitChildToTakeSiblingsAsChildren)
	            {
	            	// Codebehind logic
					var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(newChildList);
			
					// Note that this loops over the original, and passes the copy
			        foreach (var child in newChildList)
			        {
			            child.RemoveRelatedFilesFromParent(copyOfChildrenToFindRelatedFiles);
			        }
			
			        // The parent directory gets what is left over after the
			        // children take their respective 'code behinds'
			        newChildList = copyOfChildrenToFindRelatedFiles;
			        
			        // This time, 'LinkChildren(...)' is invoked
			        // in order to wire up the 'TreeViewNoType.IndexAmongSiblings'.
			        // 
			        // This index is used by the keyboard events to move
			        // throughout the tree view.
		            ChildList = newChildList;
		            LinkChildren(ChildList, ChildList);
	            }
	        }
        }
        catch (Exception exception)
        {
            ChildList = new List<TreeViewNoType>
            {
                new TreeViewException(exception, false, false, CommonComponentRenderers)
                {
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }

        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
    }

    /// <summary>
    /// This method is called on each child when loading children for a parent node.
    /// This method allows for code-behinds
    /// </summary>
    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        if (Item.AbsolutePath.ExtensionNoPeriod.EndsWith(ExtensionNoPeriodFacts.RAZOR_MARKUP))
            TreeViewHelperRazorMarkup.FindRelatedFiles(this, siblingsAndSelfTreeViews);
    }
}