using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
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
            {
                Types.Undefined.TypeIdentifier.TextSpan.GetText(),
                Types.Undefined
            },
        };

            return new BoundScope(
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