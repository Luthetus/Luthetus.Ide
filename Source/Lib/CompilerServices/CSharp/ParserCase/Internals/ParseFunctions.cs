using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        IdentifierToken consumedIdentifierToken,
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
        parserModel.CurrentCodeBlockBuilder.SetInnerPendingCodeBlockOwner(
        	createScope: false, functionDefinitionNode, compilationUnit, ref parserModel);

        if (parserModel.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = parserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

			foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		if (argument.IsOptional)
	    			compilationUnit.Binder.BindFunctionOptionalArgument(argument, compilationUnit);
	    		else
	    			compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
	    	}
        }
    }

    public static void HandleConstructorDefinition(
    	TypeDefinitionNode typeDefinitionNodeCodeBlockOwner,
        IdentifierToken consumedIdentifierToken,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit, ref parserModel);

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNodeCodeBlockOwner.TypeIdentifierToken,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            consumedIdentifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        compilationUnit.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, compilationUnit);
        parserModel.SyntaxStack.Push(constructorDefinitionNode);
        parserModel.CurrentCodeBlockBuilder.SetInnerPendingCodeBlockOwner(
        	createScope: false, constructorDefinitionNode, compilationUnit, ref parserModel);

        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
        	_ = parserModel.TokenWalker.Consume();
            // Constructor invokes some other constructor as well
        	// 'this(...)' or 'base(...)'
        	
        	KeywordToken keywordToken;
        	
        	if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
        		keywordToken = (KeywordToken)parserModel.TokenWalker.Match(SyntaxKind.ThisTokenKeyword);
        	else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.BaseTokenKeyword)
        		keywordToken = (KeywordToken)parserModel.TokenWalker.Match(SyntaxKind.BaseTokenKeyword);
        	else
        		keywordToken = default;
        		
        	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
	    	var startInclusivePreliminaryIndex = parserModel.TokenWalker.Index;
	    	
	    	var matchBraces = 0;
	    	var matchParenthesis = 1;
	    	
	    	while (!parserModel.TokenWalker.IsEof)
	    	{
	    		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
	    		{
	    			matchBraces++;
	    		}
	    		else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseBraceToken)
	    		{
	    			matchBraces--;
	    			
	    			if (matchBraces == -1)
	    				break;
	    		}
	    		else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
	    		{
	    			matchParenthesis++;
	    		}
	    		else if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
	    		{
	    			matchParenthesis--;
	    			
	    			if (matchParenthesis == 0)
	    				break;
	    		}
	    	
	    		_ = parserModel.TokenWalker.Consume();
	    	}
	    	
	    	var endExclusivePreliminaryIndex = parserModel.TokenWalker.Index;
	        
			var closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
        	
        	constructorDefinitionNode.StartInclusivePreliminaryIndex = startInclusivePreliminaryIndex;
        	constructorDefinitionNode.EndExclusivePreliminaryIndex = endExclusivePreliminaryIndex;
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static FunctionArgumentsListingNode HandleFunctionArguments(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var openParenthesisToken = (OpenParenthesisToken)parserModel.TokenWalker.Consume();
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
        
        var closeParenthesisToken = default(CloseParenthesisToken);
        
        if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	closeParenthesisToken = (CloseParenthesisToken)parserModel.TokenWalker.Consume();
        
        return new FunctionArgumentsListingNode(
        	openParenthesisToken,
	        functionArgumentEntryNodeList,
	        closeParenthesisToken);
    }
}
