using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Associated;

public class AssociatedEntryGroupBuilder
{
    public AssociatedEntryGroupBuilder(
        SyntaxToken openAssociatedGroupToken,
        Action<AssociatedEntryGroup> onAfterBuildAction)
    {
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        OnAfterBuildAction = onAfterBuildAction;
    }

    public Action<AssociatedEntryGroup> OnAfterBuildAction { get; }
    public SyntaxToken OpenAssociatedGroupToken { get; set; }
    public List<IAssociatedEntry> AssociatedEntryList { get; } = new();
    public SyntaxToken CloseAssociatedGroupToken { get; set; }

    public AssociatedEntryGroup Build()
    {
        var group = new AssociatedEntryGroup(
            OpenAssociatedGroupToken,
            AssociatedEntryList,
            CloseAssociatedGroupToken);

        OnAfterBuildAction.Invoke(group);
        return group;
    }
}
