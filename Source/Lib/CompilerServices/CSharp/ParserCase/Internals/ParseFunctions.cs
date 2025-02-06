using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        SyntaxToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericParametersListingNode? consumedGenericArgumentsListingNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    	{
    		parserModel.ParserContextKind = CSharpParserContextKind.ForceParseGenericParameters;
    		var successGenericParametersListingNode = ParseOthers.TryParseExpression(
    			SyntaxKind.GenericParametersListingNode,
    			compilationUnit,
    			ref parserModel,
    			out var genericParametersListingNode);
    			
    		if (successGenericParametersListingNode)
    			consumedGenericArgumentsListingNode = (GenericParametersListingNode)genericParametersListingNode;
    	}
    
        if (parserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit, ref parserModel);

        var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeClauseNode,
            consumedIdentifierToken,
            consumedGenericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        compilationUnit.Binder.BindFunctionDefinitionNode(functionDefinitionNode, compilationUnit);
        parserModel.SyntaxStack.Push(functionDefinitionNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	functionDefinitionNode,
	        functionDefinitionNode.GetReturnTypeClauseNode(),
	        parserModel.TokenWalker.Current.TextSpan,
	        compilationUnit,
	        ref parserModel);
        
        // (2025-01-13)
		// ========================================================
		//
		// - FunctionDefinitionNode checks encompassing CodeBlockOwner, if it is an interface. 
		//
		// - 'SetActiveCodeBlockBuilder', 'SetActiveScope', and 'PermitInnerPendingCodeBlockOwnerToBeParsed'
		//   should all be handled by the same method.
		//
		// - PermitInnerPendingCodeBlockOwnerToBeParsed needs to move
		//   to the ICodeBlockOwner itself.
		// 
		// - 'parserModel.SyntaxStack.Push(PendingCodeBlockOwner);' is unnecessary because
		//   the CodeBlockBuilder and Scope will be active.
		//
		// - '...InnerPendingCodeBlockOwner = PendingCodeBlockOwner;' needs to change
		//   to 'set active code block builder' and 'set active scope'.

        if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

			foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
	    	}
        }
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsCloseAngleBracketToken)
        {
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        
        	_ = parserModel.TokenWalker.Consume(); // Consume 'EqualsCloseAngleBracketToken'
        	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
        	parserModel.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
        }
    }

    public static void HandleConstructorDefinition(
    	TypeDefinitionNode typeDefinitionNodeCodeBlockOwner,
        SyntaxToken consumedIdentifierToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit, ref parserModel);

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

        compilationUnit.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, compilationUnit);
        parserModel.SyntaxStack.Push(constructorDefinitionNode);
        
        compilationUnit.Binder.NewScopeAndBuilderFromOwner(
        	constructorDefinitionNode,
	        constructorDefinitionNode.GetReturnTypeClauseNode(),
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
            
            var openParenthesisToken = parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
            
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
			        
			    parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, functionParametersListingNode));
				parserModel.ExpressionList.Add((SyntaxKind.ColonToken, functionParametersListingNode));
				
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
				_ = compilationUnit.Binder.ParseNamedParameterSyntaxAndReturnEmptyExpressionNode(compilationUnit, ref parserModel);
				
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
        	parserModel.CurrentCodeBlockBuilder.IsImplicitOpenCodeBlockTextSpan = true;
        
        	_ = parserModel.TokenWalker.Consume(); // Consume 'EqualsCloseAngleBracketToken'
        	var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
        	parserModel.CurrentCodeBlockBuilder.ChildList.Add(expressionNode);
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static FunctionArgumentsListingNode HandleFunctionArguments(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var openParenthesisToken = parserModel.TokenWalker.Consume();
    	var functionArgumentEntryNodeList = new List<FunctionArgumentEntryNode>();
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
		    			var identifierToken = UtilityApi.ConvertToIdentifierToken(parserModel.TokenWalker.Consume(), compilationUnit, ref parserModel);
		    			successNameableToken = true;
		    			
		    			if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    			{
		    				_ = parserModel.TokenWalker.Consume();
		    				
		    				parserModel.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
		    				parserModel.ExpressionList.Add((SyntaxKind.CommaToken, null));
		    				var expressionNode = ParseOthers.ParseExpression(compilationUnit, ref parserModel);
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
        
        return new FunctionArgumentsListingNode(
        	openParenthesisToken,
	        functionArgumentEntryNodeList,
	        closeParenthesisToken);
    }
}
