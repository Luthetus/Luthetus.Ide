using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class SyntaxReader
{
    /// <summary>
    /// This method builds a string of all <see cref="ISyntaxToken"/>(s)
    /// which make up the given <see cref="ISyntax"/>.
    /// </summary>
    public static string GetTextRecursively(this ISyntax syntax)
    {
        if (syntax is ISyntaxNode node)
        {
            var textBuilder = new StringBuilder();

            foreach (var child in node.ChildBag)
            {
                textBuilder.Append(child.GetTextRecursively());
            }

            if (syntax is FunctionDefinitionNode functionDefinitionNode)
                textBuilder.Append('\n');

            return textBuilder.ToString();
        }
        else if (syntax is ISyntaxToken token)
        {
            // TODO: Don't insert whitespace by way of 'assumption'.
            // I'm checking if the 'token.SyntaxKind == SyntaxKind.IdentifierToken'.
            // I really should be looking at the Trivia to ensure there really was a space
            // there and etc...
            var additionalEndingText = "";

            if (token.SyntaxKind == SyntaxKind.IdentifierToken)
                additionalEndingText = " ";

            return token.TextSpan.GetText() + additionalEndingText;
        }

        throw new ApplicationException(
            $"The {nameof(SyntaxKind)} of: '{syntax.SyntaxKind}' is unknown.");
    }
}
