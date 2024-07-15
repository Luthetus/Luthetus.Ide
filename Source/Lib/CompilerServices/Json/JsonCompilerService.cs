using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.Json.Json.SyntaxActors;

namespace Luthetus.CompilerServices.Json;

public sealed class JsonCompilerService : CompilerService
{
    public JsonCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new JsonResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorJsonLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new Parser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}