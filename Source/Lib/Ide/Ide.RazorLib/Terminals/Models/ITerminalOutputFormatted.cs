using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public interface ITerminalOutputFormatted
{
	public string Text { get; }
}
