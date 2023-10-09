using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public class DotNetSolutionGlobalSectionBuilder
{
    public string? GlobalSectionArgument { get; set; }
    public string? GlobalSectionOrder { get; set; }
    public List<IAssociatedEntry> AssociatedEntryBag { get; } = new();

    public DotNetSolutionGlobalSection Build()
    {
        return new DotNetSolutionGlobalSection(
            GlobalSectionArgument ?? string.Empty,
            GlobalSectionOrder ?? string.Empty,
            AssociatedEntryBag.ToImmutableArray());
    }
}