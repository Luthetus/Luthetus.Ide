using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Lexes.Models;

public class TextEditorLexerDefaultTests
{
    public TextEditorLexerDefault(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string sourceText, Key<RenderState> modelRenderStateKey)
    {
        return Task.FromResult(ImmutableArray<TextEditorTextSpan>.Empty);
    }
}