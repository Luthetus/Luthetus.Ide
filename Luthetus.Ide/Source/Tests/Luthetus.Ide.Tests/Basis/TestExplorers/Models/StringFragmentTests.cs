using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.Models;

public class StringFragmentTests
{
    [Fact]
    public void StringFragment()
    {
        //public (string stringValue)
    }

    [Fact]
    public void Value()
    {
        //public string  { get; set; }
    }

    [Fact]
    public void Map()
    {
        //public Dictionary<string, StringFragment>  { get; set; } = new();
    }

    [Fact]
    public void IsEndpoint()
    {
        //public bool  { get; set; }
    }

    [Fact]
    public void DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey()
    {
        //public Key<TerminalCommand>  { get; } = Key<TerminalCommand>.NewKey();
    }
}
