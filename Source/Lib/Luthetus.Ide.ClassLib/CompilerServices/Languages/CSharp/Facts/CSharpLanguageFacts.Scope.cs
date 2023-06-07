using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
        {
            var classMap = new Dictionary<string, BoundClassDeclarationNode>
            {
                {
                    Types.Void.TypeClauseToken.TextSpan.GetText(),
                    Types.Void
                },
                {
                    Types.Var.TypeClauseToken.TextSpan.GetText(),
                    Types.Var
                },
                {
                    Types.Bool.TypeClauseToken.TextSpan.GetText(),
                    Types.Bool
                },
                {
                    Types.Int.TypeClauseToken.TextSpan.GetText(),
                    Types.Int
                },
                {
                    Types.String.TypeClauseToken.TextSpan.GetText(),
                    Types.String
                },
            };

            return new BoundScope(
                null,
                typeof(void),
                0,
                null,
                new ResourceUri(string.Empty),
                classMap,
                new(),
                new());
        }
    }
}