using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models;

public record DotNetSolutionGlobal
{
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public OpenAssociatedGroupToken? OpenAssociatedGroupToken { get; set; }
    public ImmutableArray<DotNetSolutionGlobalSection> DotNetSolutionGlobalSectionList { get; init; } = ImmutableArray<DotNetSolutionGlobalSection>.Empty;
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; set; }
    /// <summary>
    /// If one does not find the <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/> 
    /// then <see cref="WasFound"/> is to be false. Otherwise, upon finding the 
    /// <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/>, then <see cref="WasFound"/>
    /// should be set to true.
    /// </summary>
    public bool WasFound { get; init; }
}
