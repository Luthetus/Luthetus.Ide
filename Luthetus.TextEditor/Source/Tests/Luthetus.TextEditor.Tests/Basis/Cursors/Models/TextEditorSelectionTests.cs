using Xunit;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

/// <summary>
/// <see cref="TextEditorSelection"/>
/// </summary>
public class TextEditorSelectionTests
{
    /// <summary>
    /// <see cref="TextEditorSelection(int?, int)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorSelection.AnchorPositionIndex"/>
    /// <see cref="TextEditorSelection.EndingPositionIndex"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var anchorPositionIndex = 1;
        var endingPositionIndex = 3;
        var selection = new TextEditorSelection(anchorPositionIndex, endingPositionIndex);

        Assert.Equal(anchorPositionIndex, selection.AnchorPositionIndex);
        Assert.Equal(endingPositionIndex, selection.EndingPositionIndex);

        // Use 'with' to change AnchorPositionIndex.
        {
            var outAnchorPositionIndex = selection.AnchorPositionIndex + 1;

            var outSelection = selection with
            {
                AnchorPositionIndex = outAnchorPositionIndex
            };

            // Assert the change
            Assert.Equal(outAnchorPositionIndex, outSelection.AnchorPositionIndex);
            // Assert that the old EndingPositionIndex was carried over unchanged.
            Assert.Equal(endingPositionIndex, outSelection.EndingPositionIndex);
        }

        // Use 'with' to change EndingPositionIndex.
        {
            var outEndingPositionIndex = selection.EndingPositionIndex + 1;

            var outSelection = selection with
            {
                EndingPositionIndex = outEndingPositionIndex
            };

            // Assert that the old AnchorPositionIndex was carried over unchanged.
            Assert.Equal(anchorPositionIndex, outSelection.AnchorPositionIndex);
            // Assert the change
            Assert.Equal(outEndingPositionIndex, outSelection.EndingPositionIndex);
        }
	}
}