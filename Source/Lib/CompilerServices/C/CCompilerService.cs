using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.CompilerServices.C;

public sealed class CCompilerService : CompilerService
{
    public CCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new CLexer(resource.ResourceUri, sourceText),
        };
    }
}