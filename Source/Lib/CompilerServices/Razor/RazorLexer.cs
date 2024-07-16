using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Razor.Facts;
using Luthetus.CompilerServices.Xml.Html.InjectedLanguage;
using Luthetus.CompilerServices.Xml.Html.SyntaxActors;

namespace Luthetus.CompilerServices.Razor;

public class RazorLexer : Lexer
{
    private readonly RazorCompilerService _razorCompilerService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorLexer(
        ResourceUri resourceUri,
        string sourceText,
        RazorCompilerService razorCompilerService,
        CSharpCompilerService cSharpCompilerService,
        IEnvironmentProvider environmentProvider)
        : base(
            resourceUri,
            sourceText,
            LexerKeywords.Empty)
    {
        _environmentProvider = environmentProvider;
        _razorCompilerService = razorCompilerService;
        _cSharpCompilerService = cSharpCompilerService;

        RazorSyntaxTree = new RazorSyntaxTree(ResourceUri, _razorCompilerService, _cSharpCompilerService, _environmentProvider);
    }

    public RazorSyntaxTree RazorSyntaxTree { get; private set; }

    public override void Lex()
    {
        RazorSyntaxTree = new RazorSyntaxTree(ResourceUri, _razorCompilerService, _cSharpCompilerService, _environmentProvider);

        InjectedLanguageDefinition razorInjectedLanguageDefinition = new(
            RazorFacts.TRANSITION_SUBSTRING,
            RazorFacts.TRANSITION_SUBSTRING_ESCAPED,
            RazorSyntaxTree.ParseInjectedLanguageFragment,
            RazorSyntaxTree.ParseTagName,
            RazorSyntaxTree.ParseAttributeName,
            RazorSyntaxTree.ParseAttributeValue);

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            ResourceUri,
            _stringWalker.SourceText,
            razorInjectedLanguageDefinition);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        // Tag Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.TagNameNodes.Select(tns => new BadToken(tns.TextEditorTextSpan)));

        // InjectedLanguageFragmentSyntaxes
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.InjectedLanguageFragmentNodes.Select(ilfs => new BadToken(ilfs.TextEditorTextSpan)));

        // Attribute Names
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeNameNodes.Select(an => new BadToken(an.TextEditorTextSpan)));

        // Attribute Values
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.AttributeValueNodes.Select(av => new BadToken(av.TextEditorTextSpan)));

        // Comments
        _syntaxTokenList.AddRange(
            htmlSyntaxWalker.CommentNodes.Select(c => new BadToken(c.TextEditorTextSpan)));

        RazorSyntaxTree.ParseCodebehind();
    }
}