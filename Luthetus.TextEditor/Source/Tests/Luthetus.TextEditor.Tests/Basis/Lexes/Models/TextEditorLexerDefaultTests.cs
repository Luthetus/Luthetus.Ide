using Xunit;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Lexes.Models;

/// <summary>
/// <see cref="TextEditorLexerDefault"/>
/// </summary>
public class TextEditorLexerDefaultTests
{
    /// <summary>
    /// <see cref="TextEditorLexerDefault(ResourceUri)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorLexerDefault.ModelRenderStateKey"/>
    /// <see cref="TextEditorLexerDefault.ResourceUri"/>
    /// <see cref="TextEditorLexerDefault.Lex(string, Key{RenderState})"/>
    /// </summary>
    [Fact]
	public async Task Constructor()
	{
        var resourceUri = new ResourceUri("/unitTesting.txt");
		var lexer = new TextEditorLexerDefault(resourceUri);

        Assert.Equal(Key<RenderState>.Empty, lexer.ModelRenderStateKey);
		Assert.Equal(resourceUri, lexer.ResourceUri);

        var lexResult = await lexer.Lex(string.Empty, Key<RenderState>.Empty);
        Assert.Equal(ImmutableArray<TextEditorTextSpan>.Empty, lexResult);
	}
}