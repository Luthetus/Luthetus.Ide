using CliWrap;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Clipboards.Models;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Luthetus.Ide.Tests.Basis.Clipboards.Models;

/// <summary>
/// <see cref="ClipboardFacts"/>
/// </summary>
public class ClipboardFactsTests
{
    /// <summary>
    /// <see cref="ClipboardFacts.Tag"/>
    /// </summary>
    [Fact]
    public void Tag()
    {
        Assert.Equal("`'\";luth_clipboard", ClipboardFacts.Tag);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.FieldDelimiter"/>
    /// </summary>
    [Fact]
    public void FieldDelimiter()
    {
        Assert.Equal("_", ClipboardFacts.FieldDelimiter);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.CopyCommand"/>
    /// </summary>
    [Fact]
    public void CopyCommand()
    {
        Assert.Equal("copy", ClipboardFacts.CopyCommand);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.CutCommand"/>
    /// </summary>
    [Fact]
    public void CutCommand()
    {
        Assert.Equal("cut", ClipboardFacts.CutCommand);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.AbsolutePathDataType"/>
    /// </summary>
    [Fact]
    public void AbsolutePathDataType()
    {
        Assert.Equal("absolute-file-path", ClipboardFacts.AbsolutePathDataType);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.FormatPhrase(string, string, string)"/>
    /// </summary>
    [Fact]
    public void FormatPhrase()
    {
        var environmentProvider = new InMemoryEnvironmentProvider();
        var absolutePath = environmentProvider.AbsolutePathFactory("/unitTesting.txt", false);

        var command = ClipboardFacts.CopyCommand;
        var dataType = ClipboardFacts.AbsolutePathDataType;
        var value = absolutePath.Value;

        var actualPhraseString = ClipboardFacts.FormatPhrase(
            command,
            dataType,
            value);

        var expectedPhraseString = 
            ClipboardFacts.Tag +
            ClipboardFacts.FieldDelimiter +
            command +
            ClipboardFacts.FieldDelimiter +
            dataType +
            ClipboardFacts.FieldDelimiter +
            value;

        Assert.Equal(expectedPhraseString, actualPhraseString);
    }

    /// <summary>
    /// <see cref="ClipboardFacts.TryParseString(string, out ClipboardPhrase?)"/>
    /// </summary>
    [Fact]
    public void TryParseString()
    {
        var environmentProvider = new InMemoryEnvironmentProvider();
        var absolutePath = environmentProvider.AbsolutePathFactory("/unitTesting.txt", false);

        var command = ClipboardFacts.CopyCommand;
        var dataType = ClipboardFacts.AbsolutePathDataType;
        var value = absolutePath.Value;

        var phraseString = ClipboardFacts.FormatPhrase(
            command,
            dataType,
            value);

        Assert.True(ClipboardFacts.TryParseString(phraseString, out var clipboardPhrase) &&
            clipboardPhrase is not null);

        Assert.Equal(command, clipboardPhrase!.Command);
        Assert.Equal(dataType, clipboardPhrase.DataType);
        Assert.Equal(value, clipboardPhrase.Value);
    }
}