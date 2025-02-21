using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;

namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

public interface TextEditorLexerResult
{
    public IReadOnlyList<TextEditorTextSpan> TextSpanList { get; }
    public string ResourceUri { get; }
    public Key<RenderState> ModelRenderStateKey { get; }
}