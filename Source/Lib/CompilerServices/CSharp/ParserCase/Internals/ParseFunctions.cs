using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        SyntaxToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericParametersListingNode? consumedGenericArgumentsListingNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    	{
    		parserComputation.ParserContextKind = CSharpParserContextKind.ForceParseGenericParameters;
    		var successGenericParametersListingNode = ParseOthers.TryParseExpression(
    			SyntaxKind.GenericParametersListingNode,
    			compilationUnit,
    			ref parserComputation,
    			out var genericParametersListingNode);
    			
    		if (successGenericParametersListingNode)
    			consumedGenericArgumentsListingNode = (GenericParametersListingNode)genericParametersListingNode;
    	}
    
        if (parserComputation.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit, ref parserComputation);

        var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeClauseNode,
            consumedIdentifierToken,
            consumedGenericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        parserComputation.Binder.BindFunctionDefinitionNode(functionDefinitionNode, compilationUnit, ref parserComputation);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	functionDefinitionNode,
	        functionDefinitionNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);
        
        // (2025-01-13)
		// ========================================================
		//
		// - FunctionDefinitionNode checks encompassing CodeBlockOwner, if it is an interface.
		
		// (2025-02-06)
		// ============
		// 'where' clause / other secondary syntax if there is more.
        
        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
        {
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        }
        else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
        {
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        
        	_ = parserComputation.TokenWalker.Consume(); // Consume 'EqualsCloseAngleBracketToken'
        	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
        	parserComputation.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
        }
    }

    public static void HandleConstructorDefinition(
    	TypeDefinitionNode typeDefinitionNodeCodeBlockOwner,
        SyntaxToken consumedIdentifierToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserComputation parserComputation)
    {
    	var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit, ref parserComputation);

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNodeCodeBlockOwner.TypeIdentifierToken,
            valueType: null,
            genericParametersListingNode: null,
            isKeywordType: false);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            consumedIdentifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        parserComputation.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, compilationUnit, ref parserComputation);
        
        parserComputation.Binder.NewScopeAndBuilderFromOwner(
        	constructorDefinitionNode,
	        constructorDefinitionNode.GetReturnTypeClauseNode(),
	        parserComputation.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserComputation);

        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
        	_ = parserComputation.TokenWalker.Consume();
            // Constructor invokes some other constructor as well
        	// 'this(...)' or 'base(...)'
        	
        	SyntaxToken keywordToken;
        	
        	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
        		keywordToken = parserComputation.TokenWalker.Match(SyntaxKind.ThisTokenKeyword);
        	else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.BaseTokenKeyword)
        		keywordToken = parserComputation.TokenWalker.Match(SyntaxKind.BaseTokenKeyword);
        	else
        		keywordToken = default;
        	
        	while (!parserComputation.TokenWalker.IsEof)
            {
            	// "short circuit"
            	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken ||
                    parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketEqualsToken ||
                    parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    break;
                }
                
                // Good case
                if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                {
                	break;
                }

                _ = parserComputation.TokenWalker.Consume();
            }
            
            var openParenthesisToken = parserComputation.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
            
            // Parse secondary syntax ': base(myVariable, 7)'
            if (!openParenthesisToken.IsFabricated)
            {
            	var functionParametersListingNode = new FunctionParametersListingNode(
					openParenthesisToken,
			        new List<FunctionParameterEntryNode>(),
			        closeParenthesisToken: default);
			
				var functionInvocationNode = new FunctionInvocationNode(
					consumedIdentifierToken,
			        functionDefinitionNode: null,
			        genericParametersListingNode: null,
			        functionParametersListingNode,
			        CSharpFacts.Types.Void.ToTypeClause());
			        
			    parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
				parserComputation.ExpressionList.Add((SyntaxKind.CommaToken, functionParametersListingNode));
				parserComputation.ExpressionList.Add((SyntaxKind.ColonToken, functionParametersListingNode));
				
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
				_ = parserComputation.Binder.ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserComputation);
				
				// This invocation will parse all of the parameters because the 'parserComputation.ExpressionList'
				// contains (SyntaxKind.CommaToken, functionParametersListingNode).
				//
				// Upon encountering a CommaToken the expression loop will set 'functionParametersListingNode'
				// to the primary expression, then return an EmptyExpressionNode in order to parse the next parameter.
				_ = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
            }
        }
        
        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
        {
        	parserComputation.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        
        	_ = parserComputation.TokenWalker.Consume(); // Consume 'EqualsCloseAngleBracketToken'
        	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
        	parserComputation.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static FunctionArgumentsListingNode HandleFunctionArguments(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	var openParenthesisToken = parserComputation.TokenWalker.Consume();
    	var functionArgumentEntryNodeList = new List<FunctionArgumentEntryNode>();
    	var openParenthesisCount = 1;
    	var corruptState = false;
    	
    	while (!parserComputation.TokenWalker.IsEof)
        {
        	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        	{
        		openParenthesisCount++;
        	}
        	else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	{
        		openParenthesisCount--;
        		
        		if (openParenthesisCount == 0)
        		{
        			break;
        		}
        	}
            else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                break;
            }
            else if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
            {
            	break;
            }
            else if (!corruptState)
            {
            	if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.OutTokenKeyword ||
            		parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.InTokenKeyword ||
            		parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.RefTokenKeyword ||
            		parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.ParamsTokenKeyword ||
            		parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
            	{
            		_ = parserComputation.TokenWalker.Consume();
            	}
            
            	var tokenIndexOriginal = parserComputation.TokenWalker.Index;
            	
            	parserComputation.ParserContextKind = CSharpParserContextKind.ForceParseNextIdentifierAsTypeClauseNode;
				var successTypeClauseNode = ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, compilationUnit, ref parserComputation, out var typeClauseNode);
		    	var successName = false;
		    	
		    	if (successTypeClauseNode)
		    	{
		    		var successNameableToken = false;
		    		
		    		if (UtilityApi.IsConvertibleToIdentifierToken(parserComputation.TokenWalker.Current.SyntaxKind))
		    		{
		    			var identifierToken = UtilityApi.ConvertToIdentifierToken(parserComputation.TokenWalker.Consume(), compilationUnit, ref parserComputation);
		    			successNameableToken = true;
		    			
		    			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    			{
		    				_ = parserComputation.TokenWalker.Consume();
		    				
		    				parserComputation.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		    				parserComputation.ExpressionList.Add((SyntaxKind.CommaToken, null));
		    				var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserComputation);
		    			}
					        
					    var variableDeclarationNode = new VariableDeclarationNode(
					        (TypeClauseNode)typeClauseNode,
					        identifierToken,
					        VariableKind.Local,
					        false);
		    			
		    			var functionArgumentEntryNode = new FunctionArgumentEntryNode(
					        variableDeclarationNode,
					        optionalCompileTimeConstantToken: null,
					        isOptional: false,
					        hasParamsKeyword: false,
					        hasOutKeyword: false,
					        hasInKeyword: false,
					        hasRefKeyword: false);
		    			
		    			functionArgumentEntryNodeList.Add(functionArgumentEntryNode);
		    			
		    			if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		    				_ = parserComputation.TokenWalker.Consume();
		    				
		    			if (tokenIndexOriginal < parserComputation.TokenWalker.Index)
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

            _ = parserComputation.TokenWalker.Consume();
        }
        
        var closeParenthesisToken = default(SyntaxToken);
        
        if (parserComputation.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	closeParenthesisToken = parserComputation.TokenWalker.Consume();
        
        return new FunctionArgumentsListingNode(
        	openParenthesisToken,
	        functionArgumentEntryNodeList,
	        closeParenthesisToken);
    }
}
