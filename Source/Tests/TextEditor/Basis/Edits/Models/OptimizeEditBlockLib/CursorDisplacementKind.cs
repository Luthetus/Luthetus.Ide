namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

/// <summary>
/// This enum describes which direction the cursor's column index moves based on the
/// amount of text that was inserted, deleted, or some other form of editing. <br/><br/>
///
/// <see cref="CursorDisplacementKind.Left"/> is -1 * (amount of text that was inserted, or etc...) <br/><br/>
/// <see cref="CursorDisplacementKind.None"/> is 0 * (amount of text that was inserted, or etc...) <br/><br/>
/// <see cref="CursorDisplacementKind.Right"/> is 1 * (amount of text that was inserted, or etc...) <br/><br/>
/// </summary>
/// <remarks>
/// - Given a 'delete' key edit. If one performs an 'undo' then the
/// <see cref="CursorDisplacementKind"/> of the 'insert' edit will
/// be <see cref="CursorDisplacementKind.None"/>.
///     (in the previous example the 'insert' edit is made because it is
///      the opposite action to a 'delete' edit. This therefore will satisfy the 'undo') <br/><br/>
///
/// - Given a 'backspace' key edit. If one performs an 'undo' then the
/// <see cref="CursorDisplacementKind"/> of the 'insert' edit will
/// be <see cref="CursorDisplacementKind.Right"/>. <br/><br/>
///
/// - Given an 'insert' edit. If one performs an 'undo' then the
/// <see cref="CursorDisplacementKind"/> of the 'delete' edit will
/// be <see cref="CursorDisplacementKind.Left"/>. <br/><br/>
/// </remarks>
public enum CursorDisplacementKind
{
	Left,
	None,
	Right,
}
