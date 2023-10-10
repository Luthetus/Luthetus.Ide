using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;

public class AssociatedEntryGroupBuilder
{
    public AssociatedEntryGroupBuilder(
        OpenAssociatedGroupToken openAssociatedGroupToken,
        Action<AssociatedEntryGroup> onAfterBuildAction)
    {
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        OnAfterBuildAction = onAfterBuildAction;
    }

    public Action<AssociatedEntryGroup> OnAfterBuildAction { get; }
    public OpenAssociatedGroupToken OpenAssociatedGroupToken { get; set; }
    public List<IAssociatedEntry> AssociatedEntryBag { get; } = new();
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; set; }

    public AssociatedEntryGroup Build()
    {
        var group = new AssociatedEntryGroup(
            OpenAssociatedGroupToken,
            AssociatedEntryBag.ToImmutableArray(),
            CloseAssociatedGroupToken);

        OnAfterBuildAction.Invoke(group);
        return group;
    }
}
