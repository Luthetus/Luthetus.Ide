using Luthetus.CompilerServices.Lang.TypeScript.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public sealed class TypeScriptCompilerService : LuthCompilerService
{
    public TypeScriptCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            GetLexerFunc = (resource, sourceText) => new TextEditorTypeScriptLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }

    public override void RegisterResource(ResourceUri resourceUri)
    {
        lock (_resourceMapLock)
        {
            if (_resourceMap.ContainsKey(resourceUri))
                return;

            _resourceMap.Add(
                resourceUri,
                new TypeScriptResource(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        OnResourceRegistered();
    }
}