using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class StringFragment
{
	public StringFragment(string stringValue)
	{
		Value = stringValue;
	}

	public string Value { get; set; }
	public Dictionary<string, StringFragment> Map { get; set; } = new();
	public bool IsEndpoint { get; set; }
	public TerminalCommand? TerminalCommand { get; set; }
	public Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
}
