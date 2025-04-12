using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        SyntaxToken consumedIdentifierToken,
        TypeReference consumedTypeReference,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	GenericParameterListing genericParameterListing = default;
    
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    	{
    		parserModel.ParserContextKind = CSharpParserContextKind.ForceParseGenericParameters;
    		var successGenericParametersListingNode = ParseOthers.TryParseExpression(
    			SyntaxKind.GenericParametersListingNode,
    			compilationUnit,
    			ref parserModel,
    			out var expressionNode);
    			
    		if (successGenericParametersListingNode)
    			genericParameterListing = ((IGenericParameterNode)expressionNode).GenericParameterListing;
    	}
    
        if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

		var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeReference,
            consumedIdentifierToken,
            genericParameterListing,
            functionArgumentListing: default,
            null);

        HandleFunctionArguments(functionDefinitionNode, compilationUnit, ref parserModel);

        parserModel.Binder.BindFunctionDefinitionNode(functionDefinitionNode, compilationUnit, ref parserModel);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	functionDefinitionNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
        
        // (2025-01-13)
		// ========================================================
		//
		// - FunctionDefinitionNode checks encompassing CodeBlockOwner, if it is an interface.
		
		// (2025-02-06)
		// ============
		// 'where' clause / other secondary syntax if there is more.
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
        {
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        }
        else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
        {
        	ParseTokens.MoveToExpressionBody(compilationUnit, ref parserModel);
        }
    }

    public static void HandleConstructorDefinition(
    	TypeDefinitionNode typeDefinitionNodeCodeBlockOwner,
        SyntaxToken consumedIdentifierToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	var typeClauseNode = new TypeClauseNode(
            typeDefinitionNodeCodeBlockOwner.TypeIdentifierToken,
            valueType: null,
            genericParameterListing: default,
            isKeywordType: false);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            new TypeReference(typeClauseNode),
            consumedIdentifierToken,
            default,
            functionArgumentListing: default,
            null);
    
    	HandleFunctionArguments(constructorDefinitionNode, compilationUnit, ref parserModel);

        parserModel.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, compilationUnit, ref parserModel);
        
        parserModel.Binder.NewScopeAndBuilderFromOwner(
        	constructorDefinitionNode,
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);

        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
        	_ = parserModel.TokenWalker.Consume();
            // Constructor invokes some other constructor as well
        	// 'this(...)' or 'base(...)'
        	
        	SyntaxToken keywordToken;
        	
        	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
        		keywordToken = parserModel.TokenWalker.Match(SyntaxKind.ThisTokenKeyword);
        	else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.BaseTokenKeyword)
        		keywordToken = parserModel.TokenWalker.Match(SyntaxKind.BaseTokenKeyword);
        	else
        		keywordToken = default;
        	
        	while (!parserModel.TokenWalker.IsEof)
            {
            	// "short circuit"
            	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken ||
                    parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketEqualsToken ||
                    parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    break;
                }
                
                // Good case
                if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                {
                	break;
                }

                _ = parserModel.TokenWalker.Consume();
            }
            
            // Parse secondary syntax ': base(myVariable, 7)'
            if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
            	var openParenthesisToken = parserModel.TokenWalker.Current;
            
				var functionInvocationNode = new FunctionInvocationNode(
					consumedIdentifierToken,
			        functionDefinitionNode: null,
			        genericParameterListing: default,
			        new FunctionParameterListing(
						openParenthesisToken,
				        new List<FunctionParameterEntry>(),
				        closeParenthesisToken: default),
			        CSharpFacts.Types.Void.ToTypeReference());
			        
			    functionInvocationNode.IsParsingFunctionParameters = true;
			        
			    parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionInvocationNode));
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, functionInvocationNode));
				
				// TODO: The 'ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(...)' code needs to be invoked...
				// ...from within the expression loop.
				// But, as of this comment a way to do so does not exist.
				//
				// Therefore, if the secondary constructor invocation were ': base(person: new Person())'
				// then the first named parameter would not parse correctly.
				//
				// If the second or onwards parameters were named they would be parsed correctly.
				//
				// So, explicitly adding this invocation so that the first named parameter parses correctly.
				//
				_ = parserModel.Binder.ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel, guaranteeConsume: true);
				
				// This invocation will parse all of the parameters because the 'parserModel.ExpressionList'
				// contains (SyntaxKind.CommaToken, functionParametersListingNode).
				//
				// Upon encountering a CommaToken the expression loop will set 'functionParametersListingNode'
				// to the primary expression, then return an EmptyExpressionNode in order to parse the next parameter.
				_ = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
            }
        }
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
        {
        	ParseTokens.MoveToExpressionBody(compilationUnit, ref parserModel);
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static void HandleFunctionArguments(
    	IFunctionDefinitionNode functionDefinitionNode,
    	CSharpCompilationUnit compilationUnit,
    	ref CSharpParserModel parserModel)
    {
    	var openParenthesisToken = parserModel.TokenWalker.Consume();
    	var functionArgumentEntryList = new List<FunctionArgumentEntry>();
    	var openParenthesisCount = 1;
    	var corruptState = false;
    	
    	while (!parserModel.TokenWalker.IsEof)
        {
        	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        	{
        		openParenthesisCount++;
        	}
        	else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	{
        		openParenthesisCount--;
        		
        		if (openParenthesisCount == 0)
        		{
        			break;
        		}
        	}
            else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                break;
            }
            else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
            {
            	break;
            }
            else if (!corruptState)
            {
            	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OutTokenKeyword ||
            		parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.InTokenKeyword ||
            		parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.RefTokenKeyword ||
            		parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ParamsTokenKeyword ||
            		parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
            	{
            		_ = parserModel.TokenWalker.Consume();
            	}
            
            	var tokenIndexOriginal = parserModel.TokenWalker.Index;
            	
            	parserModel.ParserContextKind = CSharpParserContextKind.ForceParseNextIdentifierAsTypeClauseNode;
				var successTypeClauseNode = ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, compilationUnit, ref parserModel, out var typeClauseNode);
		    	var successName = false;
		    	
		    	if (successTypeClauseNode)
		    	{
		    		var successNameableToken = false;
		    		
		    		if (UtilityApi.IsConvertibleToIdentifierToken(parserModel.TokenWalker.Current.SyntaxKind))
		    		{
		    			var token = parserModel.TokenWalker.Consume();
		    			var identifierToken = UtilityApi.ConvertToIdentifierToken(ref token, compilationUnit, ref parserModel);
		    			successNameableToken = true;
		    			
		    			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    			{
		    				_ = parserModel.TokenWalker.Consume();
		    				
		    				parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		    				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, null));
		    				var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
		    			}
					        
					    var variableDeclarationNode = new VariableDeclarationNode(
					        new TypeReference((TypeClauseNode)typeClauseNode),
					        identifierToken,
					        VariableKind.Local,
					        false);
		    			
		    			functionArgumentEntryList.Add(
		    				new FunctionArgumentEntry(
						        variableDeclarationNode,
						        optionalCompileTimeConstantToken: null,
						        isOptional: false,
						        hasParamsKeyword: false,
						        hasOutKeyword: false,
						        hasInKeyword: false,
						        hasRefKeyword: false));
		    			
		    			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		    				_ = parserModel.TokenWalker.Consume();
		    				
		    			if (tokenIndexOriginal < parserModel.TokenWalker.Index)
		    				continue; // Already consumed so avoid the one at the end of the while loop
		    		}
		    		
		    		if (!successNameableToken)
		    			corruptState = true;
		    	}
		    	else
		    	{
		    		corruptState = true;
		    	}
            }

            _ = parserModel.TokenWalker.Consume();
        }
        
        var closeParenthesisToken = default(SyntaxToken);
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	closeParenthesisToken = parserModel.TokenWalker.Consume();
        
        functionDefinitionNode.SetFunctionArgumentListing(
        	new FunctionArgumentListing(
	        	openParenthesisToken,
		        functionArgumentEntryList,
		        closeParenthesisToken));
    }
}
