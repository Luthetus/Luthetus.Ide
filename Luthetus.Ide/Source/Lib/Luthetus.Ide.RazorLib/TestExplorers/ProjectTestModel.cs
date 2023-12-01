using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public class ProjectTestModel
{
	public Guid ProjectIdGuid { get; set; }
	public List<string> DotNetTestListTestsCommandOutput { get; set; } = new();
	public Dictionary<string, StringFragment> RootStringFragmentMap { get; set; } = new();
	public IAbsolutePath AbsolutePath { get; set; }
	public Key<TerminalCommand> DotNetTestListTestsTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
	public Func<Task> EnqueueDiscoverTestsFunc { get; set; }
	
	public string DirectoryNameForTestDiscovery => AbsolutePath.ParentDirectory?.Value ?? string.Empty;
}
