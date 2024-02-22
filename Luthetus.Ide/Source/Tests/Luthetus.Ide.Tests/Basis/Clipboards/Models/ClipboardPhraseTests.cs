using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Clipboards.Models;

namespace Luthetus.Ide.Tests.Basis.Clipboards.Models;

/// <summary>
/// <see cref="ClipboardPhrase"/>
/// </summary>
public class ClipboardPhraseTests
{
    /// <summary>
    /// <see cref="ClipboardPhrase(string, string, string)"/>
    /// <br/>----<br/>
    /// <see cref="ClipboardPhrase.Command"/>
    /// <see cref="ClipboardPhrase.DataType"/>
    /// <see cref="ClipboardPhrase.Value"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var environmentProvider = new InMemoryEnvironmentProvider();
        var absolutePath = environmentProvider.AbsolutePathFactory("/unitTesting.txt", false);

        var command = ClipboardFacts.CopyCommand;
        var dataType = ClipboardFacts.AbsolutePathDataType;
        var value = absolutePath.Value;

        var clipboardPhrase = new ClipboardPhrase(
            command,
            dataType,
            value);

        Assert.Equal(command, clipboardPhrase!.Command);
        Assert.Equal(dataType, clipboardPhrase.DataType);
        Assert.Equal(value, clipboardPhrase.Value);
    }
}