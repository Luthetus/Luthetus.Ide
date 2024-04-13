using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : LuthCompilerService
{
    public DotNetSolutionCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new DotNetSolutionResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new DotNetSolutionLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new LuthParser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}