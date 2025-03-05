using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.GenericLexer.Decoration;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxActors;

public class TextEditorXmlLexer
{
	public static readonly LexerKeywords LexerKeywords = LexerKeywords.Empty;
	
    public TextEditorXmlLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
    }

	public ResourceUri ResourceUri { get; }
	public string SourceText { get; }

	public List<SyntaxToken> SyntaxTokenList { get; } = new();

    public void Lex()
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();
        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        // Tag Names
        SyntaxTokenList.AddRange(
            htmlSyntaxWalker.TagNameNodes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        // InjectedLanguageFragmentSyntaxes
        SyntaxTokenList.AddRange(
            htmlSyntaxWalker.InjectedLanguageFragmentNodes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        // Attribute Names
        SyntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeNameNodes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        // Attribute Values
        SyntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeValueNodes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));

        // Comments
        SyntaxTokenList.AddRange(
            htmlSyntaxWalker.CommentNodes.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextEditorTextSpan)));
            
		var endOfFileTextSpan = new TextEditorTextSpan(
            SourceText.Length,
		    SourceText.Length,
		    (byte)GenericDecorationKind.None,
		    ResourceUri,
		    SourceText,
		    getTextPrecalculatedResult: string.Empty);
		
        SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.EndOfFileToken, endOfFileTextSpan));
    }
}