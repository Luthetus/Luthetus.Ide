using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.Diff._2023_03_15;

public class DiffSingleCharacterTests
{
    [Fact]
    public void InputsAreNotEqual()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "a";

        var afterResourceUri = new ResourceUri("after");
        var afterText = "b";

        // Expected
        var expectedLongestCommonSubsequence = string.Empty;

        // Calculate
        var diffResult = TextEditorDiffResult.Calculate(
            beforeResourceUri,
            beforeText,
            afterResourceUri,
            afterText);

        // Assert
        Assert.Equal(
            expectedLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence);
    }

    [Fact]
    public void InputsAreEqual()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "a";

        var afterResourceUri = new ResourceUri("after");
        var afterText = "a";

        // Expected
        var expectedLongestCommonSubsequence = "a";

        // Calculate
        var diffResult = TextEditorDiffResult.Calculate(
            beforeResourceUri,
            beforeText,
            afterResourceUri,
            afterText);

        // Assert
        Assert.Equal(
            expectedLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence);
    }
}