using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class ProjectTestModel
{
	// I want to focus on the relevant details.
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

	// Perfect, this is exactly what we want 'Key<TerminalCommand>'.
//
// We can use this to lookup what output in the terminal was due to the specified command.
//
// The alternative to this might be getting the entirety of the output that was wrriten to the terminal,
// since the beginning of time.
	public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
	public Action<TreeViewNoType> ReRenderNodeAction { get; }
	
	public string DirectoryNameForTestDiscovery => AbsolutePath.ParentDirectory?.Value ?? string.Empty;
}
