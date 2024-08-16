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
	public TerminalCommandRequest? TerminalCommandRequest { get; set; }
	public TerminalCommandParsed? TerminalCommandParsed { get; set; }
	public Key<TerminalCommandRequest> DotNetTestByFullyQualifiedNameFormattedTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();
}
