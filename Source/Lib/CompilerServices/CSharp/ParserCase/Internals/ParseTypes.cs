using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTypes
{
    public static void HandleStaticClassIdentifier(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
    }

    public static void HandleUndefinedTypeOrNamespaceReference(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
    }

    /// <summary>
    /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
    /// </summary>
    public static void HandleGenericArguments(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
    }

    public static void HandleAttribute(
        OpenSquareBracketToken consumedOpenSquareBracketToken,
        CSharpParserModel model)
    {
    }

	/// <summary>
	/// This method should only be used to disambiguate syntax.
	/// If it is known that there has to be a TypeClauseNode at the current position,
	/// then use <see cref="MatchTypeClause"/>.
	///
	/// Example: 'someMethod(out var e);'
	///           In this example the 'out' can continue into a variable reference or declaration.
	///           Therefore an invocation to this method is performed to determine if it is a declaration.
	/// 
	/// Furthermore, because there is a need to disambiguate, more checks are performed in this method
	/// than in <see cref="MatchTypeClause"/>.
	/// </summary>
	public static bool IsPossibleTypeClause(ISyntaxToken syntaxToken, CSharpParserModel model)
	{
		return false;
	}

    public static TypeClauseNode MatchTypeClause(CSharpParserModel model)
    {
    	return TypeFacts.Empty.ToTypeClause();
    }

    public static void HandlePrimaryConstructorDefinition(
        TypeDefinitionNode typeDefinitionNode,
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
    }
}
