using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class ScopeFacts
    {
        public static Scope GetInitialGlobalScope()
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
	        
			return new Scope(
				codeBlockOwner: null,
				indexKey: 0,
			    parentIndexKey: -1,
			    0,
			    endExclusiveIndex: -1);
        }
    }
}