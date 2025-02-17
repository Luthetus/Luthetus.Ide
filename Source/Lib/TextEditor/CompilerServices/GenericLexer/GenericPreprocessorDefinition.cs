namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericPreprocessorDefinition
{
    public GenericPreprocessorDefinition(
        string transitionSubstring,
        IReadOnlyList<DeliminationExtendedSyntaxDefinition> deliminationExtendedSyntaxList)
    {
        TransitionSubstring = transitionSubstring;
        DeliminationExtendedSyntaxList = deliminationExtendedSyntaxList;
    }

    public string TransitionSubstring { get; }
    public IReadOnlyList<DeliminationExtendedSyntaxDefinition> DeliminationExtendedSyntaxList { get; }
}