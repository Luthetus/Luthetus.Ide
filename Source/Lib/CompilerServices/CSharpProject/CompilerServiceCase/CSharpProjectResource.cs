using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;

public class CSharpProjectResource : CompilerServiceResource
{
    public CSharpProjectResource(ResourceUri resourceUri, CSharpProjectCompilerService cSharpProjectCompilerService)
        : base(resourceUri, cSharpProjectCompilerService)
    {
    }
}