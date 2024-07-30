namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputFormatterExpand : ITerminalOutputFormatter
{
	public const string NAME = nameof(TerminalOutputFormatterExpand);

	public string Name { get; } = NAME;
	
	public string Format(ITerminal terminal)
	{
		return $"TODO: {nameof(TerminalOutputFormatterExpand)}";
	}
}
