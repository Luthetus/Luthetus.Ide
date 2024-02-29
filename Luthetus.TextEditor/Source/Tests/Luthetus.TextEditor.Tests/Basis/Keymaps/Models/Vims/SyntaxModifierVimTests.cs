using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

/// <summary>
/// <see cref="SyntaxModifierVim"/>
/// </summary>
public class SyntaxModifierVimTests
{
	/// <summary>
	/// <see cref="SyntaxModifierVim.TryLex(KeymapArgument, bool, out VimGrammarToken?)"/>
	/// </summary>
	[Fact]
	public void TryLex()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="SyntaxModifierVim.TryParse(TextEditorKeymapVim, ImmutableArray{VimGrammarToken}, int, KeymapArgument, bool, out TextEditorCommand?)"/>
	/// </summary>
	[Fact]
	public void TryParse()
	{
		throw new NotImplementedException();
	}
}