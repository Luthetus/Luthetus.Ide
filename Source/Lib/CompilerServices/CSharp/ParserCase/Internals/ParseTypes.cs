using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTypes
{
    /// <summary>
	/// TODO: TypeDefinitionNode(s) should use the expression loop to parse the...
	/// ...generic parameters. They currently use 'ParseTypes.HandleGenericParameters(...);'
	/// </summary>
    public static GenericParameterListing HandleGenericParameters(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var openAngleBracketToken = parserModel.TokenWalker.Consume();
    
    	if (SyntaxKind.CloseAngleBracketToken == parserModel.TokenWalker.Current.SyntaxKind)
        {
            return new GenericParameterListing(
                openAngleBracketToken,
                new(),
                parserModel.TokenWalker.Consume());
        }

        var genericParameterList = new List<GenericParameterEntry>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = MatchTypeClause(compilationUnit, ref parserModel);

            if (typeClauseNode.IsFabricated)
                break;

            var genericArgumentEntryNode = new GenericParameterEntry(new TypeReference(typeClauseNode));
            genericParameterList.Add(genericArgumentEntryNode);

            if (SyntaxKind.CommaToken == parserModel.TokenWalker.Current.SyntaxKind)
            {
                var commaToken = parserModel.TokenWalker.Consume();

                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeAngleBracketToken = parserModel.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        return new GenericParameterListing(
            openAngleBracketToken,
            genericParameterList,
            closeAngleBracketToken);
    }

    public static TypeClauseNode MatchTypeClause(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, compilationUnit, ref parserModel, out var expressionNode))
    	{
    		return (TypeClauseNode)expressionNode;
    	}
    	else
    	{
    		var syntaxToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
    		
    		return parserModel.ConstructOrRecycleTypeClauseNode(
	            syntaxToken,
	            valueType: null,
	            genericParameterListing: default,
	            isKeywordType: false);
    	}
    	
        /*ISyntaxToken syntaxToken;
		
		if (UtilityApi.IsKeywordSyntaxKind(parserModel.TokenWalker.Current.SyntaxKind) &&
                (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(parserModel.TokenWalker.Current.SyntaxKind) ||
                UtilityApi.IsVarContextualKeyword(compilationUnit, parserModel.TokenWalker.Current.SyntaxKind)))
		{
            syntaxToken = parserModel.TokenWalker.Consume();
        }
        else
        {
            syntaxToken = parserModel.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        var typeClauseNode = new TypeClauseNode(
            syntaxToken,
            null,
            null);

        parserModel.Binder.BindTypeClauseNode(typeClauseNode, compilationUnit);

        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
        	var genericParametersListingNode = (GenericParametersListingNode)ParseOthers.Force_ParseExpression(
        		SyntaxKind.GenericParametersListingNode,
        		compilationUnit);
        		
            typeClauseNode.SetGenericParametersListingNode(genericParametersListingNode);
        }
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken)
        {
        	typeClauseNode.HasQuestionMark = true;
        	_ = parserModel.TokenWalker.Consume();
		}
        
        while (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
        {
            var openSquareBracketToken = parserModel.TokenWalker.Consume();
            var closeSquareBracketToken = parserModel.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

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
        ref CSharpParserModel parserModel)
    {
    	ParseFunctions.HandleFunctionArguments(typeDefinitionNode, compilationUnit, ref parserModel);
    	
    	if (typeDefinitionNode.PrimaryConstructorFunctionArgumentListing.ConstructorWasInvoked)
    	{
    		foreach (var argument in typeDefinitionNode.PrimaryConstructorFunctionArgumentListing.FunctionArgumentEntryList)
	    	{
	    		parserModel.Binder.CreateVariableSymbol(argument.VariableDeclarationNode.IdentifierToken, argument.VariableDeclarationNode.VariableKind, compilationUnit, ref parserModel);
	    		argument.VariableDeclarationNode.VariableKind = VariableKind.Property;
	    		parserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit, ref parserModel, shouldCreateVariableSymbol: false);
	    		parserModel.CurrentCodeBlockBuilder.AddChild(argument.VariableDeclarationNode);
	    	}
    	}
    }
    
    public static void HandleEnumDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	while (!parserModel.TokenWalker.IsEof)
    	{
    		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
    			break;
    			
    		_ = parserModel.TokenWalker.Consume();
    	}
    	
    	parserModel.CurrentCodeBlockBuilder.PermitCodeBlockParsing = true;
    	
    	parserModel.StatementBuilder.FinishStatement(parserModel.TokenWalker.Index, compilationUnit, ref parserModel);
					
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = true;
		#endif
		
		var openBraceToken = parserModel.TokenWalker.Consume();
		
		#if DEBUG
		parserModel.TokenWalker.SuppressProtectedSyntaxKindConsumption = false;
		#endif
		
        ParseTokens.ParseOpenBraceToken(openBraceToken, compilationUnit, ref parserModel);
        
        var shouldFindIdentifier = true;
        
        while (!parserModel.TokenWalker.IsEof)
    	{
    		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
    			break;
    			
    		var token = parserModel.TokenWalker.Consume();
    		
    		if (shouldFindIdentifier)
    		{
    			if (UtilityApi.IsConvertibleToIdentifierToken(token.SyntaxKind))
				{
					var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
					
					var variableDeclarationNode = new VariableDeclarationNode(
				        typeDefinitionNode.ToTypeReference(),
				        identifierToken,
				        VariableKind.EnumMember,
				        false);
				        
				    parserModel.CurrentCodeBlockBuilder.AddChild(variableDeclarationNode);
				        
				    parserModel.Binder.BindEnumMember(variableDeclarationNode, compilationUnit, ref parserModel);
					
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
