using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public class CSharpProjectResource : LuthCompilerServiceResource
{
    public CSharpProjectResource(ResourceUri resourceUri, CSharpProjectCompilerService cSharpProjectCompilerService)
        : base(resourceUri, cSharpProjectCompilerService)
    {
    }
}