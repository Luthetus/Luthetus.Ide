using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Namespaces
    {
        private const string SystemNamespaceIdentifier = "System";

        /// <summary>TODO: Implement the initial bound namespaces correctly. Until then just add the "System" namespace as a nonsensical node.</summary>
        public static Dictionary<string, BoundNamespaceStatementNode> GetInitialBoundNamespaceStatementNodes()
        {
            return new Dictionary<string, BoundNamespaceStatementNode>
            {
                {
                    SystemNamespaceIdentifier,
                    new BoundNamespaceStatementNode(
                        new(new(0, 0, 0, new(string.Empty), string.Empty)),
                        new(new(0, SystemNamespaceIdentifier.Length, 0, new(string.Empty), SystemNamespaceIdentifier)),
                        System.Collections.Immutable.ImmutableArray<BoundNamespaceEntryNode>.Empty)
                }
            };
        }
    }
}