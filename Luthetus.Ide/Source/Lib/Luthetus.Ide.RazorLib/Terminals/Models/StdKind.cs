namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>
/// Rename the 'Std' prefix that keeps getting used in this folder. I don't like it.
/// </summary>
public enum StdKind
{
	StdErr,
	StdInRequest,
	StdOut,
	StdQuiescent,
}
