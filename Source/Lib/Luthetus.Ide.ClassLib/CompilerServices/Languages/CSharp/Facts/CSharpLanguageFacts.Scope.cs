using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
        {
            var typeMap = new Dictionary<string, Type>
            {
                {
                    Types.Void.name,
                    Types.Void.type
                },
                {
                    Types.Var.name,
                    Types.Var.type
                },
                {
                    Types.Bool.name,
                    Types.Bool.type
                },
                {
                    Types.Int.name,
                    Types.Int.type
                },
                {
                    Types.String.name,
                    Types.String.type
                },
            };

            return new BoundScope(
                null,
                typeof(void),
                0,
                null,
                new ResourceUri(string.Empty),
                typeMap,
                new(),
                new(),
                new());
        }
    }
}