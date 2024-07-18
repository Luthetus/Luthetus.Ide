using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.C;

public class CResource : CompilerServiceResource
{
    public CResource(ResourceUri resourceUri, CCompilerService cCompilerService)
        : base(resourceUri, cCompilerService)
    {
    }
}