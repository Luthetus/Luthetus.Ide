using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

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
                        new NamespaceStatementNode[] 
                        {
                            new NamespaceStatementNode(
                                new(new(0, 0, 0, new(string.Empty), string.Empty), SyntaxKind.UnrecognizedTokenKeyword),
                                new(new(0, SystemNamespaceIdentifier.Length, 0, new(string.Empty), SystemNamespaceIdentifier)),
                                new CodeBlockNode(ImmutableArray<ISyntax>.Empty))
                        }.ToImmutableArray())
                }
            };
        }
        
        public static NamespaceStatementNode GetTopLevelNamespaceStatementNode()
        {
            return new NamespaceStatementNode(
                new(new(0, 0, 0, new(string.Empty), string.Empty), SyntaxKind.UnrecognizedTokenKeyword),
                new(new(0, TopLevelNamespaceIdentifier.Length, 0, new(string.Empty), TopLevelNamespaceIdentifier)),
                new CodeBlockNode(ImmutableArray<ISyntax>.Empty));
        }
    }
}