using System.Text;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

public partial class TerminalCommandParsedExpandDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TerminalCommandParsed TerminalCommandParsed { get; set; } = null!;
	[Parameter, EditorRequired]
	public StringBuilder OutputBuilder { get; set; } = null!;
	
	private bool _isExpanded;
	
	private void ToggleIsExpanded()
	{
		_isExpanded = !_isExpanded;
	}
}