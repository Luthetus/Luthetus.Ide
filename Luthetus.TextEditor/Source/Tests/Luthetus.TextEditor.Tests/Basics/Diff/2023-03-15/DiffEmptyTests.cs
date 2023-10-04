using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.Diff._2023_03_15;

public class DiffEmptyTests
{
    [Fact]
    public void BeforeIsEmptyAfterIsEmpty()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = string.Empty;

        var afterResourceUri = new ResourceUri("after");
        var afterText = string.Empty;

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
    public void BeforeIsEmptyAfterIsNotEmpty()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = string.Empty;

        var afterResourceUri = new ResourceUri("after");
        var afterText = "lorem ipsum";

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
    public void BeforeIsNotEmptyAfterIsEmpty()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "lorem ipsum";

        var afterResourceUri = new ResourceUri("after");
        var afterText = string.Empty;

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
}