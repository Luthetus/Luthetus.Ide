using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

/// <summary>
/// <see cref="SyntaxVerbVim"/>
/// </summary>
public class SyntaxVerbVimTests
{
	/// <summary>
	/// <see cref="SyntaxVerbVim.TryLex(KeymapArgument, bool, out VimGrammarToken?)"./>
	/// </summary>
	[Fact]
	public void TryLex()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="SyntaxVerbVim.TryParse(TextEditorKeymapVim, ImmutableArray{VimGrammarToken}, int, KeymapArgument, bool, out TextEditorCommand?)"./>
	/// </summary>
	[Fact]
	public void TryParse()
	{
		throw new NotImplementedException();
	}
}