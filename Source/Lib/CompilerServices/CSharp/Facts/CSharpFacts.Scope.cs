using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.Facts;

public partial class CSharpFacts
{
    public class ScopeFacts
    {
        public static IScope GetInitialGlobalScope()
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
				Key<IScope>.Empty,
			    parentKey: null,
			    ResourceUri.Empty,
			    0,
			    endingIndexExclusive: null);
        }
    }
}