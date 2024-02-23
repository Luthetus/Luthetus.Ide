using Luthetus.CompilerServices.Lang.TypeScript.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public sealed class TypeScriptCompilerService : LuthCompilerService
{
    public TypeScriptCompilerService(
            ITextEditorService textEditorService)
        : base(
            textEditorService,
            (resourceUri, sourceText) => new TextEditorTypeScriptLexer(resourceUri, sourceText),
            lexer => new LuthParser(lexer))
    {
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

            QueueParseRequest(resourceUri);
        }

        OnResourceRegistered();
    }
}