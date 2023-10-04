using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.Tests.TestDataFolder;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.Lexers;

public class LexPlainTests
{
    [Fact]
    public async Task LexNothing()
    {
        var text = TestData.Plain.EXAMPLE_TEXT_25_LINES
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var defaultLexer = new TextEditorLexerDefault(resourceUri);

        var textEditorTextSpans = await defaultLexer.Lex(
            text,
            Key<RenderState>.NewKey());

        Assert.Empty(textEditorTextSpans);
    }
}