using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class Scope
    {
        public static CSharpBoundScope GetInitialGlobalScope()
        {
            var typeDefinitionMap = new Dictionary<string, TypeDefinitionNode>
        {
            {
                Types.Void.TypeIdentifierToken.TextSpan.GetText(),
                Types.Void
            },
            {
                Types.Var.TypeIdentifierToken.TextSpan.GetText(),
                Types.Var
            },
            {
                Types.Bool.TypeIdentifierToken.TextSpan.GetText(),
                Types.Bool
            },
            {
                Types.Int.TypeIdentifierToken.TextSpan.GetText(),
                Types.Int
            },
            {
                Types.String.TypeIdentifierToken.TextSpan.GetText(),
                Types.String
            },
        };

            return new CSharpBoundScope(
                null,
                Types.Void.ToTypeClause(),
                0,
                null,
                new ResourceUri(string.Empty),
                typeDefinitionMap,
                new(),
                new(),
                Namespaces.GetTopLevelNamespaceStatementNode(),
                new());
        }
    }
}