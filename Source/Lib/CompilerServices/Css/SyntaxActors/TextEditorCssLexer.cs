using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Css.SyntaxActors;

public class TextEditorCssLexer
{
	private static readonly LexerKeywords LexerKeywords = LexerKeywords.Empty;

    public TextEditorCssLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
    }
    
    public List<SyntaxToken> SyntaxTokenList { get; } = new();
    
    public ResourceUri ResourceUri { get; set; }
    public string SourceText { get; set; }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var syntaxWalker = new CssSyntaxWalker();
        syntaxWalker.Visit(syntaxNodeRoot);

        SyntaxTokenList.AddRange(
            syntaxWalker.IdentifierSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.CommentSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.PropertyNameSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.PropertyValueSyntaxes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));
    }
}