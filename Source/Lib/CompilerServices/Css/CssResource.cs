using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Css;

public class CssResource : CompilerServiceResource
{
    public CssResource(ResourceUri resourceUri, CssCompilerService textEditorCssCompilerService)
        : base(resourceUri, textEditorCssCompilerService)
    {
    }
}