using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.Facts;

public partial class CLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
        {
            var classMap = new Dictionary<string, BoundClassDefinitionNode>
            {
                {
                    Types.Int.TypeClauseToken.TextSpan.GetText(),
                    Types.Int
                },
                {
                    Types.String.TypeClauseToken.TextSpan.GetText(),
                    Types.String
                },
                {
                    Types.Void.TypeClauseToken.TextSpan.GetText(),
                    Types.Void
                }
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