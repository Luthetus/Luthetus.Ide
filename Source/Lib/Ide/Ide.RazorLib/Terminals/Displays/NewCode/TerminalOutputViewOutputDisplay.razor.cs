using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class TerminalOutputViewOutputDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TerminalOutput TerminalOutput { get; set; } = null!;
	[Parameter, EditorRequired]
	public TerminalCommandParsed TerminalCommandParsed { get; set; } = null!;
}