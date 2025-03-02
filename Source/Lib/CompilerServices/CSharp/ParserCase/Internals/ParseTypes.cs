using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTypes
{
    /// <summary>
    /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
    /// 
    /// Retrospective: What is this code??? It isn't correct and it should probably just invoke the expression logic that will parse generics.
    /// </summary>
    public static GenericArgumentsListingNode HandleGenericArguments(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var openAngleBracketToken = parserComputation.TokenWalker.Consume();
    
    	if (SyntaxKind.CloseAngleBracketToken == parserComputation.TokenWalker.Current.SyntaxKind)
        {
            return new GenericArgumentsListingNode(
                openAngleBracketToken,
                GenericArgumentsListingNode.__empty,
                parserComputation.TokenWalker.Consume());
        }

        var mutableGenericArgumentsListing = new List<GenericArgumentEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = MatchTypeClause(compilationUnit, ref parserComputation);

            if (typeClauseNode.IsFabricated)
                break;

            var genericArgumentEntryNode = new GenericArgumentEntryNode(typeClauseNode);
            mutableGenericArgumentsListing.Add(genericArgumentEntryNode);

            if (SyntaxKind.CommaToken == parserComputation.TokenWalker.Current.SyntaxKind)
            {
                var commaToken = parserComputation.TokenWalker.Consume();

                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeAngleBracketToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        return new GenericArgumentsListingNode(
            openAngleBracketToken,
            mutableGenericArgumentsListing,
            closeAngleBracketToken);
    }

    public static TypeClauseNode MatchTypeClause(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, compilationUnit, ref parserComputation, out var expressionNode))
    	{
    		return (TypeClauseNode)expressionNode;
    	}
    	else
    	{
    		var syntaxToken = parserComputation.TokenWalker.Match(SyntaxKind.IdentifierToken);
    		
    		return new TypeClauseNode(
	            syntaxToken,
	            valueType: null,
	            genericParametersListingNode: null,
	            isKeywordType: false);
    	}
    	
        /*ISyntaxToken syntaxToken;
		
		if (UtilityApi.IsKeywordSyntaxKind(parserComputation.TokenWalker.Current.SyntaxKind) &&
                (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(parserComputation.TokenWalker.Current.SyntaxKind) ||
                UtilityApi.IsVarContextualKeyword(compilationUnit, parserComputation.TokenWalker.Current.SyntaxKind)))
		{
            syntaxToken = parserComputation.TokenWalker.Consume();
        }
        else
        {
            syntaxToken = parserComputation.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        var typeClauseNode = new TypeClauseNode(
            syntaxToken,
            null,
            null);

        parserComputation.Binder.BindTypeClauseNode(typeClauseNode, compilationUnit);

        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
        	var genericParametersListingNode = (GenericParametersListingNode)ParseOthers.Force_ParseExpression(
        		SyntaxKind.GenericParametersListingNode,
        		compilationUnit);
        		
            typeClauseNode.SetGenericParametersListingNode(genericParametersListingNode);
        }
        
        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken)
        {
        	typeClauseNode.HasQuestionMark = true;
        	_ = parserComputation.TokenWalker.Consume();
		}
        
        while (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
        {
            var openSquareBracketToken = parserComputation.TokenWalker.Consume();
            var closeSquareBracketToken = parserComputation.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

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
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserComputation parserComputation)
    {
    	var functionArgumentsListingNode = ParseFunctions.HandleFunctionArguments(compilationUnit, ref parserComputation);
    	typeDefinitionNode.SetPrimaryConstructorFunctionArgumentsListingNode(functionArgumentsListingNode);
    	
    	if (typeDefinitionNode.PrimaryConstructorFunctionArgumentsListingNode is not null)
    	{
    		foreach (var argument in typeDefinitionNode.PrimaryConstructorFunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
	    	}
    	}
    }
    
    public static void HandleEnumDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserComputation parserComputation)
    {
    	while (!parserComputation.TokenWalker.IsEof)
    	{
    		if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
    			break;
    			
    		_ = parserComputation.TokenWalker.Consume();
    	}
    	
    	parserComputation.CurrentCodeBlockBuilder.PermitCodeBlockParsing = true;
    	
    	parserComputation.StatementBuilder.FinishStatement(parserComputation.TokenWalker.Index, compilationUnit, ref parserComputation);
					
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		var openBraceToken = parserComputation.TokenWalker.Consume();
		
		#if DEBUG
		parserComputation.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
		
        ParseTokens.ParseOpenBraceToken(openBraceToken, compilationUnit, ref parserComputation);
        
        var shouldFindIdentifier = true;
        
        while (!parserComputation.TokenWalker.IsEof)
    	{
    		if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
    			break;
    			
    		var token = parserComputation.TokenWalker.Consume();
    		
    		if (shouldFindIdentifier)
    		{
    			if (UtilityApi.IsConvertibleToIdentifierToken(token.SyntaxKind))
				{
					var identifierToken = UtilityApi.ConvertToIdentifierToken(token, compilationUnit, ref parserComputation);
					
					var variableDeclarationNode = new VariableDeclarationNode(
				        typeDefinitionNode.ToTypeClause(),
				        identifierToken,
				        VariableKind.EnumMember,
				        false);
				        
				    parserComputation.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);
				        
				    compilationUnit.Binder.BindEnumMember(variableDeclarationNode, compilationUnit, ref parserComputation);
					
					shouldFindIdentifier = !shouldFindIdentifier;
	    		}
    		}
    		else
    		{
    			if (token.SyntaxKind == SyntaxKind.CommaToken)
    				shouldFindIdentifier = !shouldFindIdentifier;
    		}
    	}
    }
}
