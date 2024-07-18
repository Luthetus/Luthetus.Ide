using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.Razor.CompilerServiceCase;

public sealed class RazorCompilerService : CompilerService
{
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorCompilerService(
            ITextEditorService textEditorService,
            CSharpCompilerService cSharpCompilerService,
            IEnvironmentProvider environmentProvider)
        : base(textEditorService)
    {
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;

        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new RazorResource(resourceUri, this, _textEditorService),
            GetLexerFunc = (resource, sourceText) => 
            {
                ((RazorResource)resource).HtmlSymbols.Clear();

                return new RazorLexer(
                    resource.ResourceUri,
                    sourceText,
                    this,
                    _cSharpCompilerService,
                    _environmentProvider);
            },
            GetParserFunc = null,
            GetBinderFunc = null,
            OnAfterLexAction = (resource, lexer) =>
            {
                var razorResource = (RazorResource)resource;
                var razorLexer = (RazorLexer)lexer;

                razorResource.SyntaxTokenList = razorLexer.SyntaxTokenList;
                razorResource.RazorSyntaxTree = razorLexer.RazorSyntaxTree;
            },
            OnAfterParseAction = null,
        };
    }
}