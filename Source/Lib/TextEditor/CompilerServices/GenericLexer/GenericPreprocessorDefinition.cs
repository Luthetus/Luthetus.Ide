using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericPreprocessorDefinition
{
    public GenericPreprocessorDefinition(
        string transitionSubstring,
        ImmutableArray<DeliminationExtendedSyntaxDefinition> deliminationExtendedSyntaxList)
    {
        TransitionSubstring = transitionSubstring;
        DeliminationExtendedSyntaxList = deliminationExtendedSyntaxList;
    }

    public string TransitionSubstring { get; }
    public ImmutableArray<DeliminationExtendedSyntaxDefinition> DeliminationExtendedSyntaxList { get; }
}