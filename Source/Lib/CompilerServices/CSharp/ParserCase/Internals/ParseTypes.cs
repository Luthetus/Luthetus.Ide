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
    public static GenericArgumentsListingNode HandleGenericArguments(CSharpParserModel model)
    {
    	var openAngleBracketToken = (OpenAngleBracketToken)model.TokenWalker.Consume();
    
    	if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            return new GenericArgumentsListingNode(
                openAngleBracketToken,
                ImmutableArray<GenericArgumentEntryNode>.Empty,
                (CloseAngleBracketToken)model.TokenWalker.Consume());
        }

        var mutableGenericArgumentsListing = new List<GenericArgumentEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = MatchTypeClause(model);

            if (typeClauseNode.IsFabricated)
                break;

            var genericArgumentEntryNode = new GenericArgumentEntryNode(typeClauseNode);
            mutableGenericArgumentsListing.Add(genericArgumentEntryNode);

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();

                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        return new GenericArgumentsListingNode(
            openAngleBracketToken,
            mutableGenericArgumentsListing.ToImmutableArray(),
            closeAngleBracketToken);
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
    	if (ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, model, out var expressionNode))
    	{
    		return (TypeClauseNode)expressionNode;
    	}
    	else
    	{
    		var syntaxToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
    		
    		return new TypeClauseNode(
	            syntaxToken,
	            null,
	            null);
    	}
    	
        /*ISyntaxToken syntaxToken;
		
		if (UtilityApi.IsKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) &&
                (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) ||
                UtilityApi.IsVarContextualKeyword(model, model.TokenWalker.Current.SyntaxKind)))
		{
            syntaxToken = model.TokenWalker.Consume();
        }
        else
        {
            syntaxToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        var typeClauseNode = new TypeClauseNode(
            syntaxToken,
            null,
            null);

        model.Binder.BindTypeClauseNode(typeClauseNode, model);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
        	var genericParametersListingNode = (GenericParametersListingNode)ParseOthers.Force_ParseExpression(
        		SyntaxKind.GenericParametersListingNode,
        		model);
        		
            typeClauseNode.SetGenericParametersListingNode(genericParametersListingNode);
        }
        
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken)
        {
        	typeClauseNode.HasQuestionMark = true;
        	_ = model.TokenWalker.Consume();
		}
        
        while (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
        {
            var openSquareBracketToken = model.TokenWalker.Consume();
            var closeSquareBracketToken = model.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

            var arraySyntaxTokenTextSpan = syntaxToken.TextSpan with
            {
                EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
            };

            var arraySyntaxToken = new ArraySyntaxToken(arraySyntaxTokenTextSpan);
            var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);

            var genericParametersListingNode = new GenericParametersListingNode(
                new OpenAngleBracketToken(openSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                },
                new List<GenericParameterEntryNode> { genericParameterEntryNode },
                new CloseAngleBracketToken(closeSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                });

            return new TypeClauseNode(
                arraySyntaxToken,
                null,
                genericParametersListingNode);

            // TODO: Implement multidimensional arrays. This array logic always returns after finding the first array syntax.
        }

        return typeClauseNode;
        */
    }

    public static void HandlePrimaryConstructorDefinition(
        TypeDefinitionNode typeDefinitionNode,
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
    }
}
