using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer;

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