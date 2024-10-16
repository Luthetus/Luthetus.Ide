using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.CompilerServices.DotNetSolution.Models.Associated;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

public class DotNetSolutionGlobalSectionBuilder
{
    public AssociatedValueToken? GlobalSectionArgument { get; set; }
    public AssociatedValueToken? GlobalSectionOrder { get; set; }
    public AssociatedEntryGroup AssociatedEntryGroup { get; set; } = new(openAssociatedGroupToken: default, ImmutableArray<IAssociatedEntry>.Empty, closeAssociatedGroupToken: default);

    public DotNetSolutionGlobalSection Build()
    {
        return new DotNetSolutionGlobalSection(
            GlobalSectionArgument,
            GlobalSectionOrder,
            AssociatedEntryGroup);
    }
}