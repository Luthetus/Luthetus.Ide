using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;

public class IdeCSharpLexer : ITextEditorLexer
{
    private readonly object _lexerLock = new object();

    public IdeCSharpLexer(ResourceUri resourceUri)
    {
        ResourceUri = resourceUri;
    }

    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;
    public Lexer? RecentLexSession { get; private set; }

    public ResourceUri ResourceUri { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string sourceText,
        RenderStateKey modelRenderStateKey)
    {
        Lexer? lexSession;

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

        lexSession = new Lexer(
            ResourceUri,
            sourceText);

        lexSession.Lex();

        RecentLexSession = lexSession;

        return Task.FromResult(
            lexSession.SyntaxTokens.Select(x => x.TextSpan)
            .ToImmutableArray());
    }
}
