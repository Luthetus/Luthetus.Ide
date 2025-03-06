using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

public record DotNetSolutionGlobal
{
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public SyntaxToken? OpenAssociatedGroupToken { get; set; }
    public List<DotNetSolutionGlobalSection> DotNetSolutionGlobalSectionList { get; init; } = new();
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public SyntaxToken? CloseAssociatedGroupToken { get; set; }
    /// <summary>
    /// If one does not find the <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/> 
    /// then <see cref="WasFound"/> is to be false. Otherwise, upon finding the 
    /// <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/>, then <see cref="WasFound"/>
    /// should be set to true.
    /// </summary>
    public bool WasFound { get; init; }
}
