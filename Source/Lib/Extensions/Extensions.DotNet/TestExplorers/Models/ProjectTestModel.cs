using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class ProjectTestModel
{
	public ProjectTestModel(
		Guid projectIdGuid,
		IAbsolutePath absolutePath,
		Func<Func<Dictionary<string, StringFragment>, Task>, Task> enqueueDiscoverTestsFunc,
		Action<TreeViewNoType> reRenderNodeAction)
	{
		ProjectIdGuid = projectIdGuid;
		AbsolutePath = absolutePath;
		EnqueueDiscoverTestsFunc = enqueueDiscoverTestsFunc;
		ReRenderNodeAction = reRenderNodeAction;
	}

	public List<string>? DotNetTestListTestsCommandOutput { get; set; }
	public Func<Func<Dictionary<string, StringFragment>, Task>, Task> EnqueueDiscoverTestsFunc { get; set; }
	public Dictionary<string, StringFragment> RootStringFragmentMap { get; set; } = new();

	public Guid ProjectIdGuid { get; }
	public IAbsolutePath AbsolutePath { get; }
	public TerminalCommand? TerminalCommand { get; set; }
	public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
	public Action<TreeViewNoType> ReRenderNodeAction { get; }

	public string DirectoryNameForTestDiscovery => AbsolutePath.ParentDirectory?.Value ?? string.Empty;
}
