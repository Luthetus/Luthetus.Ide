using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.Facts;

public partial class CLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
        {
            var typeMap = new Dictionary<string, Type>
            {
                {
                    Types.Int.name,
                    Types.Int.type
                },
                {
                    Types.String.name,
                    Types.String.type
                },
                {
                    Types.Void.name,
                    Types.Void.type
                }
            };

            return new BoundScope(
                null,
                typeof(void),
                0,
                null,
                typeMap,
                new(),
                new(),
                new());
        }
    }
}