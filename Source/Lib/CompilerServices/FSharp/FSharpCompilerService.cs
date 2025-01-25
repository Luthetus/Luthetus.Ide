using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.FSharp.SyntaxActors;

namespace Luthetus.CompilerServices.FSharp;

public sealed class FSharpCompilerService : CompilerService
{
    public FSharpCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new FSharpResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorFSharpLexer(resource.ResourceUri, sourceText),
        };
    }
}