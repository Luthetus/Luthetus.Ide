using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Scope
    {
        public static CSharpBoundScope GetInitialGlobalScope()
        {
            var typeDefinitionMap = new Dictionary<string, TypeDefinitionNode>
        {
            {
                Types.Void.TypeIdentifier.TextSpan.GetText(),
                Types.Void
            },
            {
                Types.Var.TypeIdentifier.TextSpan.GetText(),
                Types.Var
            },
            {
                Types.Bool.TypeIdentifier.TextSpan.GetText(),
                Types.Bool
            },
            {
                Types.Int.TypeIdentifier.TextSpan.GetText(),
                Types.Int
            },
            {
                Types.String.TypeIdentifier.TextSpan.GetText(),
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
                new());
        }
    }
}