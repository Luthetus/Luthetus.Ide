using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Css;

public class CssResource : CompilerServiceResource
{
    public CssResource(ResourceUri resourceUri, CssCompilerService textEditorCssCompilerService)
        : base(resourceUri, textEditorCssCompilerService)
    {
    }
}