using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.Xml.Html.SyntaxActors;

namespace Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;

public sealed class CSharpProjectCompilerService : CompilerService
{
    public CSharpProjectCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CSharpProjectResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorXmlLexer(resource.ResourceUri, sourceText),
        };
    }
}