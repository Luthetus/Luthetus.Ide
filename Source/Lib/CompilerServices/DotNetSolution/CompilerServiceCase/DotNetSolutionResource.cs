using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;

public class DotNetSolutionResource : LuthCompilerServiceResource
{
    public DotNetSolutionResource(ResourceUri resourceUri, DotNetSolutionCompilerService dotNetSolutionCompilerService)
        : base(resourceUri, dotNetSolutionCompilerService)
    {
    }
}