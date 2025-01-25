using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;

namespace Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : CompilerService
{
    public DotNetSolutionCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new DotNetSolutionResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new DotNetSolutionLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new DotNetSolutionParser((DotNetSolutionLexer)lexer),
        };
    }
}