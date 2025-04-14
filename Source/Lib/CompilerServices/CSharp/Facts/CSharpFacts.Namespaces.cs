using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class Namespaces
    {
        private const string SystemNamespaceIdentifier = "System";

        public const string TopLevelNamespaceIdentifier = "GetTopLevelNamespaceStatementNode";

        /// <summary>TODO: Implement the initial bound namespaces correctly. Until then just add the "System" namespace as a nonsensical node.</summary>
        public static Dictionary<string, NamespaceGroup> GetInitialBoundNamespaceStatementNodes()
        {
            return new Dictionary<string, NamespaceGroup>
            {
                {
                    SystemNamespaceIdentifier,
                    new NamespaceGroup(SystemNamespaceIdentifier,
                        new List<NamespaceStatementNode>
                        {
                            new NamespaceStatementNode(
                                new(SyntaxKind.UnrecognizedTokenKeyword, new(0, 0, 0, ResourceUri.Empty, string.Empty)),
                                new(SyntaxKind.IdentifierToken, new(0, SystemNamespaceIdentifier.Length, 0, ResourceUri.Empty, SystemNamespaceIdentifier)),
                                new CodeBlock(Array.Empty<ISyntax>()))
                        })
                }
            };
        }
        
        public static NamespaceStatementNode GetTopLevelNamespaceStatementNode()
        {
            return new NamespaceStatementNode(
                new(SyntaxKind.UnrecognizedTokenKeyword, new(0, 0, 0, ResourceUri.Empty, string.Empty)),
                new(SyntaxKind.IdentifierToken, new(0, TopLevelNamespaceIdentifier.Length, 0, ResourceUri.Empty, TopLevelNamespaceIdentifier)),
                new CodeBlock(Array.Empty<ISyntax>()));
        }
    }
}