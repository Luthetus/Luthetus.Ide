using Luthetus.CompilerServices.Lang.TypeScript.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public sealed class TypeScriptCompilerService : CompilerService
{
    public TypeScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new TypeScriptResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorTypeScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new Parser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}