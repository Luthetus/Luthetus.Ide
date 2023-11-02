using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.Diff._2023_03_15;

public class DiffInsertionTests
{
    [Fact]
    public void InsertionOfSingleCharacterAtStartOfBeforeText()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "foo";

        var afterResourceUri = new ResourceUri("after");
        var afterText = "afoo";

        // Expected
        var expectedLongestCommonSubsequence = "foo";

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
    public void InsertionOfSingleCharacterAtEndOfBeforeText()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "foo";

        var afterResourceUri = new ResourceUri("after");
        var afterText = "fooa";

        // Expected
        var expectedLongestCommonSubsequence = "foo";

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
    public void InsertionOfSingleCharacterBetweenTwoExistingCharacters()
    {
        // Input
        var beforeResourceUri = new ResourceUri("before");
        var beforeText = "foo";

        var afterResourceUri = new ResourceUri("after");
        var afterText = "foao";

        // Expected
        var expectedLongestCommonSubsequence = "foo";

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