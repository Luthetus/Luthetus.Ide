using Luthetus.CompilerServices.Lang.FSharp.FSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.CompilerServices.Lang.FSharp;

public sealed class FSharpCompilerService : LuthCompilerService
{
    public FSharpCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new FSharpResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorFSharpLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}