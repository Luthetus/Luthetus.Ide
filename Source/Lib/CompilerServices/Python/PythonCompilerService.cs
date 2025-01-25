using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.CompilerServices.Python;

public sealed class PythonCompilerService : CompilerService
{
    public PythonCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new PythonResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new PythonLexer(resource.ResourceUri, sourceText),
        };
    }
}