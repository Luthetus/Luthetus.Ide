using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Namespaces
    {
        /// <summary>TODO: Implement the initial bound namespaces correctly. Until then just add the "System" namespace as a nonsensical node.</summary>
        public static Dictionary<string, BoundNamespaceStatementNode> GetInitialBoundNamespaceStatementNodes()
        {
            return new Dictionary<string, BoundNamespaceStatementNode>
            {
                {
                    "System",
                    new BoundNamespaceStatementNode(
                        new(new(0, 0, 0, new(string.Empty), string.Empty)),
                        new(new(0, 0, 0, new(string.Empty), string.Empty)),
                        System.Collections.Immutable.ImmutableArray<Common.General.CompilationUnit>.Empty)
                }
            };
        }
    }
}