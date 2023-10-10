using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models;

public class DotNetSolutionGlobalSectionBuilder
{
    public AssociatedValueToken? GlobalSectionArgument { get; set; }
    public AssociatedValueToken? GlobalSectionOrder { get; set; }
    public AssociatedEntryGroup AssociatedEntryGroup { get; set; } = new(null, ImmutableArray<IAssociatedEntry>.Empty, null);

    public DotNetSolutionGlobalSection Build()
    {
        return new DotNetSolutionGlobalSection(
            GlobalSectionArgument,
            GlobalSectionOrder,
            AssociatedEntryGroup);
    }
}