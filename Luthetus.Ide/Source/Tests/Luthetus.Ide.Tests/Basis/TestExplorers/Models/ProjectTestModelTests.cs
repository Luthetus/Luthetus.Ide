using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.Models;

public class ProjectTestModelTests
{
    [Fact]
    public void Constructor()
    {
	  //  public ProjectTestModel(
			//Guid projectIdGuid,
			//IAbsolutePath absolutePath,
			//Func<Func<Dictionary<string, StringFragment>, Task>, Task> enqueueDiscoverTestsFunc,
			//Action<TreeViewNoType> reRenderNodeAction)
    }

    [Fact]
    public void DotNetTestListTestsCommandOutput()
    {
	    //public List<string>?  { get; set; }
    }

    [Fact]
    public void EnqueueDiscoverTestsFunc()
    {
        //public Func<Func<Dictionary<string, StringFragment>, Task>, Task>  { get; set; }
    }

    [Fact]
    public void RootStringFragmentMap()
    {
        //public Dictionary<string, StringFragment>  { get; set; } = new();
    }

    [Fact]
    public void ProjectIdGuid()
    {
        //public Guid  { get; }
    }

    [Fact]
    public void AbsolutePath()
    {
        //public IAbsolutePath  { get; }
    }

    [Fact]
    public void DotNetTestListTestsTerminalCommandKey()
    {
        //public Key<TerminalCommand>  { get; } = Key<TerminalCommand>.NewKey();
    }

    [Fact]
    public void ReRenderNodeAction()
    {
        //public Action<TreeViewNoType>  { get; }
    }

    [Fact]
    public void DirectoryNameForTestDiscovery()
    {
        //public string  => AbsolutePath.ParentDirectory?.Value ?? string.Empty;
    }
}
