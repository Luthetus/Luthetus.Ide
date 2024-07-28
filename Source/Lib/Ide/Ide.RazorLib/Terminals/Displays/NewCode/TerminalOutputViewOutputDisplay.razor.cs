using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class TerminalOutputViewOutputDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TerminalOutputTextEditorExpand TerminalOutputTextEditorExpand { get; set; } = null!;
	[Parameter, EditorRequired]
	public TerminalCommandParsed TerminalCommandParsed { get; set; } = null!;
}