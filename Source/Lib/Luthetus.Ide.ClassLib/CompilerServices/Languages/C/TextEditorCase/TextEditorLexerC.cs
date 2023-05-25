using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.LexerCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.TextEditorCase;

public class TextEditorLexerC : ITextEditorLexer
{
    private readonly object _lexerLock = new object();

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;
    public LexerSession? RecentLexSession { get; private set; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        LexerSession? lexSession;

        lock (_lexerLock)
        {
            lexSession = RecentLexSession;

            if (ModelRenderStateKey == modelRenderStateKey &&
                lexSession is not null)
            {
                return Task.FromResult(
                    lexSession.SyntaxTokens.Select(x => x.TextSpan)
                    .ToImmutableArray());
            }

            ModelRenderStateKey = modelRenderStateKey;
        }

        lexSession = new LexerSession(text);

        lexSession.Lex();

        RecentLexSession = lexSession;

        return Task.FromResult(
            lexSession.SyntaxTokens.Select(x => x.TextSpan)
            .ToImmutableArray());
    }
}
