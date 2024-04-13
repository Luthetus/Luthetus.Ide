using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Css.Css.SyntaxActors;

public class TextEditorCssLexer : LuthLexer
{
    public TextEditorCssLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            LuthLexerKeywords.Empty)
    {
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var syntaxWalker = new CssSyntaxWalker();
        syntaxWalker.Visit(syntaxNodeRoot);

        _syntaxTokenList.AddRange(
            syntaxWalker.IdentifierSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.CommentSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.PropertyNameSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.PropertyValueSyntaxes.Select(x => new BadToken(x.TextEditorTextSpan)));
    }
}