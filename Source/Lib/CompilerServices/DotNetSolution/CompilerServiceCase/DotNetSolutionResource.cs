using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;

public class DotNetSolutionResource : CompilerServiceResource
{
    public DotNetSolutionResource(ResourceUri resourceUri, DotNetSolutionCompilerService dotNetSolutionCompilerService)
        : base(resourceUri, dotNetSolutionCompilerService)
    {
    }
}