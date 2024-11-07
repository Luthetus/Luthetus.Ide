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
        GenericArgumentsListingNode? consumedGenericArgumentsListingNode,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Next.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        HandleFunctionArguments(
            (OpenParenthesisToken)model.TokenWalker.Consume(),
            model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

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
        model.CurrentCodeBlockBuilder.PendingChild = functionDefinitionNode;

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

			foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		model.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, model);
	    		
	    		/*if (argument.IsOptional)
	    			model.Binder.BindFunctionOptionalArgument(argument, model);
	    		else
	    			model.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, model);*/
	    	}
        }
    }

    public static void HandleConstructorDefinition(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
    	HandleFunctionArguments(
            (OpenParenthesisToken)model.TokenWalker.Consume(),
            model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

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
        model.CurrentCodeBlockBuilder.PendingChild = constructorDefinitionNode;

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
        	
        	if (!keywordToken.ConstructorWasInvoked || keywordToken.IsFabricated)
        	{
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
        	else
        	{
        		_ = model.TokenWalker.Consume();
        		model.ExpressionList.Add((SyntaxKind.CloseParenthesisToken, null));
        		model.ExpressionList.Add((SyntaxKind.OpenBraceToken, null));
        		model.ExpressionList.Add((SyntaxKind.EqualsToken, null));
        		var functionInvocationNode = ParseOthers.ParseExpression(model);
        	}
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static void HandleFunctionArguments(
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Peek(0).SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionArgumentsListingNode(
                consumedOpenParenthesisToken,
                ImmutableArray<FunctionArgumentEntryNode>.Empty,
                (CloseParenthesisToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableFunctionArgumentListing = new List<FunctionArgumentEntryNode>();

        while (true)
        {
            var hasParamsKeyword = false;
            var hasOutKeyword = false;
            var hasInKeyword = false;
            var hasRefKeyword = false;

            // Check for keywords: { 'params', 'out', 'in', 'ref', }
            // Ignore any unmatched keywords
            {
            	if (UtilityApi.IsKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) &&
            		!UtilityApi.IsTypeIdentifierKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind))
            	{
            		switch (model.TokenWalker.Current.SyntaxKind)
            		{
            			case SyntaxKind.ParamsTokenKeyword:
            				hasParamsKeyword = true;
            				break;
            			case SyntaxKind.OutTokenKeyword:
            				hasOutKeyword = true;
            				break;
            			case SyntaxKind.InTokenKeyword:
            				hasInKeyword = true;
            				break;
            			case SyntaxKind.RefTokenKeyword:
            				hasRefKeyword = true;
            				break;
            		}

            		_ = model.TokenWalker.Consume();
            	}
            }

            // TypeClause
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (typeClauseNode.IsFabricated)
                break;

            // Identifier
            var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (variableIdentifierToken.IsFabricated)
                break;

            var variableDeclarationStatementNode = new VariableDeclarationNode(
                typeClauseNode,
                variableIdentifierToken,
                VariableKind.Local,
                false);

			// Moved binding to be in during parsing of OpenBraceToken (2024-10-08)
			
			FunctionArgumentEntryNode functionArgumentEntryNode;

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
            {
            	functionArgumentEntryNode = new FunctionArgumentEntryNode(
		            variableDeclarationStatementNode,
		            optionalCompileTimeConstantToken: null,
		            isOptional: true,
		            hasParamsKeyword,
		            hasOutKeyword,
		            hasInKeyword,
		            hasRefKeyword);
		            
		        while (!model.TokenWalker.IsEof)
		        {
		        	if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken ||
		        		model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken|| 
		        		model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
		        		model.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
		        	{
		        		break;
		        	}
		        
		        	_ = model.TokenWalker.Consume();
		        }
            }
            else
            {
            	functionArgumentEntryNode = new FunctionArgumentEntryNode(
	                variableDeclarationStatementNode,
	                optionalCompileTimeConstantToken: null,
	                false,
	                hasParamsKeyword,
	                hasOutKeyword,
	                hasInKeyword,
	                hasRefKeyword);
            }

            mutableFunctionArgumentListing.Add(functionArgumentEntryNode);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
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

        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionArgumentsListingNode(
            consumedOpenParenthesisToken,
            mutableFunctionArgumentListing.ToImmutableArray(),
            closeParenthesisToken));
    }
}
