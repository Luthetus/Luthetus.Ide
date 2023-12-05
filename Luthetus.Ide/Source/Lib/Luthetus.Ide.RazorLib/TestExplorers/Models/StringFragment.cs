using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class StringFragment
{
	public StringFragment(string stringValue)
	{
		Value = stringValue;
	}

	public string Value { get; set; }
	public Dictionary<string, StringFragment> Map { get; set; } = new();
	public bool IsEndpoint { get; set; }
	public Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
}
