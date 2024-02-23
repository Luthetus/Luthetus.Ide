using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : LuthCompilerService
{
    public DotNetSolutionCompilerService(
            ITextEditorService textEditorService)
        : base(
            textEditorService,
            (resourceUri, sourceText) => new DotNetSolutionLexer(resourceUri, sourceText),
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
                new DotNetSolutionResource(resourceUri, this));

            QueueParseRequest(resourceUri);
        }

        OnResourceRegistered();
    }
}