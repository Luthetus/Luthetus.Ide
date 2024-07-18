using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;

namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

public interface TextEditorLexerResult
{
    public ImmutableArray<TextEditorTextSpan> TextSpanList { get; }
    public string ResourceUri { get; }
    public Key<RenderState> ModelRenderStateKey { get; }
}