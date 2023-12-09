using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericPreprocessorDefinitionTests
{
    public GenericPreprocessorDefinition(
        string transitionSubstring,
        ImmutableArray<DeliminationExtendedSyntaxDefinition> deliminationExtendedSyntaxBag)
    {
        TransitionSubstring = transitionSubstring;
        DeliminationExtendedSyntaxBag = deliminationExtendedSyntaxBag;
    }

    public string TransitionSubstring { get; }
    public ImmutableArray<DeliminationExtendedSyntaxDefinition> DeliminationExtendedSyntaxBag { get; }
}