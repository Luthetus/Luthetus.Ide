using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

/// <summary>
/// <see cref="VimMotionResult"/>
/// </summary>
public class VimMotionResultTests
{
    /// <summary>
    /// <see cref="VimMotionResult(TextEditorCursor, int, TextEditorCursor, int, int)"/>
	/// <br/>----<br/>
	/// <see cref="VimMotionResult.LowerPositionIndexImmutableCursor"/>
	/// <see cref="VimMotionResult.LowerPositionIndex"/>
	/// <see cref="VimMotionResult.HigherPositionIndexImmutableCursor"/>
	/// <see cref="VimMotionResult.HigherPositionIndex"/>
	/// <see cref="VimMotionResult.PositionIndexDisplacement"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="VimMotionResult.GetResultAsync(ITextEditorModel, TextEditorCursorModifier, Func{Task})"/>
	/// </summary>
	[Fact]
	public void GetResultAsync()
	{
		throw new NotImplementedException();
	}
}