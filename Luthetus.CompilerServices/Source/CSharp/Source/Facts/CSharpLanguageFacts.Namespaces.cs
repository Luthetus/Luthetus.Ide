using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Namespaces
    {
        private const string SystemNamespaceIdentifier = "System";

        /// <summary>TODO: Implement the initial bound namespaces correctly. Until then just add the "System" namespace as a nonsensical node.</summary>
        public static Dictionary<string, NamespaceStatementNode> GetInitialBoundNamespaceStatementNodes()
        {
            return new Dictionary<string, NamespaceStatementNode>
        {
            {
                SystemNamespaceIdentifier,
                new NamespaceStatementNode(
                    new(new(0, 0, 0, new(string.Empty), string.Empty), SyntaxKind.UnrecognizedTokenKeyword),
                    new(new(0, SystemNamespaceIdentifier.Length, 0, new(string.Empty), SystemNamespaceIdentifier)),
                    System.Collections.Immutable.ImmutableArray<NamespaceEntryNode>.Empty)
            }
        };
        }
    }
}