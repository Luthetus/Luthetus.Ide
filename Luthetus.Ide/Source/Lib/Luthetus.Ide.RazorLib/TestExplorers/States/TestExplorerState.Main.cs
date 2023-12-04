using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

[FeatureState]
public partial record TestExplorerState
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState()
    {
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelBag { get; init; } = ImmutableList<ProjectTestModel>.Empty;
}