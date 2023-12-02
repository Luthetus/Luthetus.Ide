using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public class ProjectTestModel
{
	public Guid ProjectIdGuid { get; set; }
	public List<string> DotNetTestListTestsCommandOutput { get; set; } = new();
	public Dictionary<string, StringFragment> RootStringFragmentMap { get; set; } = new();
	public IAbsolutePath AbsolutePath { get; set; }
	public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
	public Func<Func<Dictionary<string, StringFragment>, Task>, Task> EnqueueDiscoverTestsFunc { get; set; }
	public Action<TreeViewNoType> ReRenderNodeAction { get; set; }
	
	public string DirectoryNameForTestDiscovery => AbsolutePath.ParentDirectory?.Value ?? string.Empty;
}
