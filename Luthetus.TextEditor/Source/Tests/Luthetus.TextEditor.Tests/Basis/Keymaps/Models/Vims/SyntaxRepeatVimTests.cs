using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

/// <summary>
/// <see cref="SyntaxRepeatVim"/>
/// </summary>
public class SyntaxRepeatVimTests
{
	/// <summary>
	/// <see cref="SyntaxRepeatVim.TryLex(KeymapArgument, bool, out VimGrammarToken?)"/>
	/// </summary>
	[Fact]
	public void TryLex()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="SyntaxRepeatVim.TryParse(TextEditorKeymapVim, ImmutableArray{VimGrammarToken}, int, KeymapArgument, bool, out TextEditorCommand?)"/>
	/// </summary>
	[Fact]
	public void TryParse()
	{
		throw new NotImplementedException();
	}
}
