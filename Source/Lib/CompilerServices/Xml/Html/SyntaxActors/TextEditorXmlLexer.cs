using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxActors;

public class TextEditorXmlLexer : Lexer
{
    public TextEditorXmlLexer(
            ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            LexerKeywords.Empty)
    {
    }

    public override void Lex()
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();
        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        // Tag Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.TagNameNodes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        // InjectedLanguageFragmentSyntaxes
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.InjectedLanguageFragmentNodes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        // Attribute Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeNameNodes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        // Attribute Values
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeValueNodes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));

        // Comments
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.CommentNodes.Select(x => (ISyntaxToken)new BadToken(x.TextEditorTextSpan)));
            
		var endOfFileTextSpan = new TextEditorTextSpan(
            SourceText.Length,
		    SourceText.Length,
		    (byte)GenericDecorationKind.None,
		    ResourceUri,
		    SourceText,
		    getTextPrecalculatedResult: string.Empty);
		
        _syntaxTokenList.Add(new EndOfFileToken(endOfFileTextSpan));
    }
}