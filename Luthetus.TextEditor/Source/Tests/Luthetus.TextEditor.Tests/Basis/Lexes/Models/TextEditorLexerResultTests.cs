using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Lexes.Models;

public interface TextEditorLexerResultTests
{
    public ImmutableArray<TextEditorTextSpan> TextSpanBag { get; }
    public string ResourceUri { get; }
    public Key<RenderState> ModelRenderStateKey { get; }
}