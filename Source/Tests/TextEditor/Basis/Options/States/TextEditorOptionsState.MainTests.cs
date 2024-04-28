using Luthetus.TextEditor.RazorLib.Options.States;

namespace Luthetus.TextEditor.Tests.Basis.Options.States;

/// <summary>
/// <see cref="TextEditorOptionsState"/>
/// </summary>
public class TextEditorOptionsStateMainTests
{
	/// <summary>
	/// <see cref="TextEditorOptionsState()"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		var optionsState = new TextEditorOptionsState();
		Assert.NotNull(optionsState.Options);
	}

	/// <summary>
	/// <see cref="TextEditorOptionsState.Options"/>
	/// </summary>
	[Fact]
	public void Options()
	{
        var optionsState = new TextEditorOptionsState();
		Assert.NotNull(optionsState.Options);

		var outCursorWidthInPixels = optionsState.Options.CursorWidthInPixels + 1;

		var outOptions = optionsState.Options with
		{
			CursorWidthInPixels = outCursorWidthInPixels
        };

		Assert.NotEqual(outOptions, optionsState.Options);

		var outOptionsState = new TextEditorOptionsState
		{
			Options = outOptions
		};
		Assert.Equal(outOptions, outOptionsState.Options);
    }
}