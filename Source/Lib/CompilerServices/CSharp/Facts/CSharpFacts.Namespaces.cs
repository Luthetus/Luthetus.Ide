using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class Namespaces
    {
        private const string SystemNamespaceIdentifier = "System";

        public const string TopLevelNamespaceIdentifier = "GetTopLevelNamespaceStatementNode";

        /// <summary>TODO: Implement the initial bound namespaces correctly. Until then just add the "System" namespace as a nonsensical node.</summary>
        public static Dictionary<string, NamespaceGroupNode> GetInitialBoundNamespaceStatementNodes()
        {
            return new Dictionary<string, NamespaceGroupNode>
            {
                {
                    SystemNamespaceIdentifier,
                    new NamespaceGroupNode(SystemNamespaceIdentifier,
                        new List<NamespaceStatementNode>
                        {
                            new NamespaceStatementNode(
                                new(new(0, 0, 0, ResourceUri.Empty, string.Empty), SyntaxKind.UnrecognizedTokenKeyword),
                                new(new(0, SystemNamespaceIdentifier.Length, 0, ResourceUri.Empty, SystemNamespaceIdentifier)),
                                new CodeBlockNode(Array.Empty<ISyntax>()))
                        })
                }
            };
        }
        
        public static NamespaceStatementNode GetTopLevelNamespaceStatementNode()
        {
            return new NamespaceStatementNode(
                new(new(0, 0, 0, ResourceUri.Empty, string.Empty), SyntaxKind.UnrecognizedTokenKeyword),
                new(new(0, TopLevelNamespaceIdentifier.Length, 0, ResourceUri.Empty, TopLevelNamespaceIdentifier)),
                new CodeBlockNode(Array.Empty<ISyntax>()));
        }
    }
}