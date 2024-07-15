using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.Css.SyntaxActors;

namespace Luthetus.CompilerServices.Css;

public sealed class CssCompilerService : CompilerService
{
    public CssCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CssResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorCssLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new Parser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}