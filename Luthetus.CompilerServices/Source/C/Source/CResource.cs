using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.C;

public class CResource : LuthCompilerServiceResource
{
    public CResource(ResourceUri resourceUri, CCompilerService cCompilerService)
        : base(resourceUri, cCompilerService)
    {
    }
}