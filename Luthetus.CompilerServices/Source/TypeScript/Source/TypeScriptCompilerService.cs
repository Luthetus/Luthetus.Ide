using Luthetus.CompilerServices.Lang.TypeScript.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public sealed class TypeScriptCompilerService : LuthCompilerService
{
    public TypeScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new TypeScriptResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorTypeScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}