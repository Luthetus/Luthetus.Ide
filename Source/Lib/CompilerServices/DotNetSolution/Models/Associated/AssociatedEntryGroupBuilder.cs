using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
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
    public List<IAssociatedEntry> AssociatedEntryList { get; } = new();
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; set; }

    public AssociatedEntryGroup Build()
    {
        var group = new AssociatedEntryGroup(
            OpenAssociatedGroupToken,
            AssociatedEntryList.ToImmutableArray(),
            CloseAssociatedGroupToken);

        OnAfterBuildAction.Invoke(group);
        return group;
    }
}
