using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        IdentifierToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericParametersListingNode? consumedGenericArgumentsListingNode,
        CSharpParserModel model)
    {
    	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    	{
    		var successGenericParametersListingNode = ParseOthers.TryParseExpression(
    			SyntaxKind.GenericParametersListingNode,
    			model,
    			out var genericParametersListingNode);
    			
    		if (successGenericParametersListingNode)
    			consumedGenericArgumentsListingNode = (GenericParametersListingNode)genericParametersListingNode;
    	}
    
        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        var functionArgumentsListingNode = HandleFunctionArguments(model);

        var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeClauseNode,
            consumedIdentifierToken,
            consumedGenericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindFunctionDefinitionNode(functionDefinitionNode, model);
        model.SyntaxStack.Push(functionDefinitionNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = functionDefinitionNode;

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

			foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		if (argument.IsOptional)
	    			model.Binder.BindFunctionOptionalArgument(argument, model);
	    		else
	    			model.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, model);
	    	}
        }
    }

    public static void HandleConstructorDefinition(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
    	var functionArgumentsListingNode = HandleFunctionArguments(model);

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is not TypeDefinitionNode typeDefinitionNode)
        {
            model.DiagnosticBag.ReportConstructorsNeedToBeWithinTypeDefinition(consumedIdentifierToken.TextSpan);
            typeDefinitionNode = Facts.CSharpFacts.Types.Void;
        }

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNode.TypeIdentifierToken,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            consumedIdentifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, model);
        model.SyntaxStack.Push(constructorDefinitionNode);
        model.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = constructorDefinitionNode;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
        	_ = model.TokenWalker.Consume();
            // Constructor invokes some other constructor as well
        	// 'this(...)' or 'base(...)'
        	
        	KeywordToken keywordToken;
        	
        	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
        		keywordToken = (KeywordToken)model.TokenWalker.Match(SyntaxKind.ThisTokenKeyword);
        	else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.BaseTokenKeyword)
        		keywordToken = (KeywordToken)model.TokenWalker.Match(SyntaxKind.BaseTokenKeyword);
        	else
        		keywordToken = default;
        	
        	while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                _ = model.TokenWalker.Consume();
            }
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static FunctionArgumentsListingNode HandleFunctionArguments(CSharpParserModel model)
    {
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Consume();
    	var functionArgumentEntryNodeList = new List<FunctionArgumentEntryNode>();
    	var openParenthesisCount = 1;
    	var corruptState = false;
    	
    	while (!model.TokenWalker.IsEof)
        {
        	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        	{
        		openParenthesisCount++;
        	}
        	else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	{
        		openParenthesisCount--;
        		
        		if (openParenthesisCount == 0)
        		{
        			break;
        		}
        	}
            else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                break;
            }
            else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken &&
            		 model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
            	break;
            }
            else if (!corruptState)
            {
            	var originalTokenIndex = model.TokenWalker.Index;
				var successTypeClauseNode = ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, model, out var typeClauseNode);
		    	var successName = false;
		    	
		    	if (successTypeClauseNode)
		    	{
		    		// 'TypeClauseNode' or 'VariableDeclarationNode'
		    		var successNameableToken = false;
		    		
		    		if (UtilityApi.IsConvertibleToIdentifierToken(model.TokenWalker.Current.SyntaxKind))
		    		{
		    			var identifierToken = UtilityApi.ConvertToIdentifierToken(model.TokenWalker.Consume(), model);
		    			successNameableToken = true;
		    			
		    			if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    			{
		    				// Optional
		    			}
		    			
		    			var variableKind = VariableKind.Local;
		    			
		    			var variableDeclarationNode = ParseVariables.HandleVariableDeclarationExpression(
					        (TypeClauseNode)typeClauseNode,
		    				identifierToken,
					        variableKind,
					        model);
		    			
		    			var functionArgumentEntryNode = new FunctionArgumentEntryNode(
					        (VariableDeclarationNode)variableDeclarationNode,
					        optionalCompileTimeConstantToken: null,
					        isOptional: false,
					        hasParamsKeyword: false,
					        hasOutKeyword: false,
					        hasInKeyword: false,
					        hasRefKeyword: false);
		    			
		    			functionArgumentEntryNodeList.Add(functionArgumentEntryNode);
		    			
		    			if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		    				_ = model.TokenWalker.Consume();
		    		}
		    		
		    		if (!successNameableToken)
		    			corruptState = true;
		    	}
		    	else
		    	{
		    		corruptState = true;
		    	}
            }

            _ = model.TokenWalker.Consume();
        }
        
        var closeParenthesisToken = default(CloseParenthesisToken);
        
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Consume();
        
        return new FunctionArgumentsListingNode(
        	openParenthesisToken,
	        functionArgumentEntryNodeList,
	        closeParenthesisToken);
    }
}
